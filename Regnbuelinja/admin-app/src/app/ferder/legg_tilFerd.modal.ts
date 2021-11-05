import { Component, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Baat } from '../models/baat';
import { Rute } from '../models/rute';
import { Ferd } from '../models/ferd';
import { AlertLagreModal } from '../modals/alert-lagre.modal';

@Component({
  templateUrl: './legg_tilFerd.modal.html',
})
export class LeggTilFerdModal {
  forms: FormGroup;
  fb: FormBuilder = new FormBuilder();
  alleBaater: Array<Baat> = [];
  alleRuter: Array<Rute> = [];

  //Formater dato
  dateArray: Array<string> = [];
  datoSplittet: Array<string> = [];
  tidSplittet: Array<string> = [];
  dato: Date;
  isoDato: string = '';

  allForms = {
    bIdForm: [null, Validators.required],
    rIdForm: [null, Validators.required],
    avreiseDatoForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(
          /(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d/
        ),
      ]),
    ],
    avreiseKlokkeslettForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(/(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]/),
      ]),
    ],
    ankomstDatoForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(
          /(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d/
        ),
      ]),
    ],
    ankomstKlokkeslettForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(/(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]/),
      ]),
    ],
  };

  constructor(
    public modal: NgbActiveModal,
    public modalService: NgbModal,
    private _http: HttpClient
  ) {}

  ngOnInit() {
    this.forms = this.fb.group(this.allForms);
    this.hentAlleRuter();
    this.hentAlleBaater();
  }

  formaterDato(datoString: string) {
    //Splitter til to deler. Del 1 = dato, del 2 = tid
    this.dateArray = datoString.split(' ', 2);

    //Splitter dato i tre deler ved "/"
    this.datoSplittet = this.dateArray[0].split('/', 3);

    //Splitter tid i to deler ved ":"
    this.tidSplittet = this.dateArray[1].split(':', 2);

    //Lager ny dato med datostring
    this.dato = new Date(
      parseInt(this.datoSplittet[2]),
      parseInt(this.datoSplittet[1]) - 1,
      parseInt(this.datoSplittet[0])
    );

    //Legger til tid-string
    this.dato.setHours(parseInt(this.tidSplittet[0]));
    this.dato.setMinutes(parseInt(this.tidSplittet[1]));

    //konverterer hele avreisedato + tid til isoString som server må ha
    return (this.isoDato = this.dato.toISOString());
  }

  vedSubmit() {
    const bId = this.forms.value.bIdForm;
    const rId = this.forms.value.rIdForm;
    const avreiseDato = this.forms.value.avreiseDatoForm;
    const ankomstDato = this.forms.value.ankomstDatoForm;
    const avreiseKlokkeslett = this.forms.value.avreiseKlokkeslettForm;
    const ankomstKlokkeslett = this.forms.value.ankomstKlokkeslettForm;

    const avreiseTid = avreiseDato + ' ' + avreiseKlokkeslett;
    const ankomstTid = ankomstDato + ' ' + ankomstKlokkeslett;

    const ferd = new Ferd(
      bId,
      rId,
      this.formaterDato(avreiseTid),
      this.formaterDato(ankomstTid)
    );

    this._http.post('/api/admin/ferder', ferd).subscribe(
      (ok) => {
        if (ok) this.modal.close('Vellykket');
        else {
          this.modal.close('Mislykket');
        }
      },
      (res) => {
        console.log(res.error);
        //AlertModal
        const modalRef = this.modalService.open(AlertLagreModal, {
          backdrop: 'static',
          keyboard: false,
        });
        let textBody: string = 'Ankomstdato må være senere enn avreisedato';
        modalRef.componentInstance.updateBody(textBody);
      }
    );
  }

  hentAlleBaater() {
    this._http.get<Baat[]>('/api/admin/baater').subscribe(
      (baater) => {
        this.alleBaater = baater;
      },
      (error) => console.log(error)
    );
  }

  hentAlleRuter() {
    this._http.get<Rute[]>('/api/admin/ruter').subscribe(
      (rutene) => {
        this.alleRuter = rutene;
      },
      (error) => console.log(error)
    );
  }
}
