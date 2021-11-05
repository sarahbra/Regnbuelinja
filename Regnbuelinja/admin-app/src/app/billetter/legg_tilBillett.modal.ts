import { Component, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FerdRute } from '../models/ferdRute';
import { Bestilling } from '../models/bestilling';
import { Billett } from '../models/billett';
import { AlertLagreModal } from '../modals/alert-lagre.modal';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: './legg_tilBillett.modal.html',
})
export class LeggTilBillettModal {
  forms: FormGroup;
  fb: FormBuilder = new FormBuilder();
  voksen: any;
  alleFerder: Array<FerdRute> = [];
  alleBestillinger: Array<Bestilling> = [];
  valgtBestilling: any;

  allForms = {
    fIdForm: [null, Validators.required],
    bIdForm: [null, Validators.required],
  };

  constructor(
    public modal: NgbActiveModal,
    private _http: HttpClient,
    public modalService: NgbModal
  ) {}

  ngOnInit() {
    this.forms = this.fb.group(this.allForms);
    this.voksen = document.getElementById('voksen');
    this.voksen.checked = true;
    this.hentAlleBestillinger();
  }

  vedSubmit() {
    const fId = this.forms.value.fIdForm;
    const bId = this.forms.value.bIdForm.id;
    console.log(fId);
    console.log(bId);
    const billett = new Billett(fId, bId, this.voksen.checked);

    this._http.post('/api/admin/billetter', billett).subscribe(
      (ok) => {
        if (ok) {
          this.modal.close('Vellykket');
        } else {
          this.modal.close('Mislykket');
        }
      },
      (res) => {
        //AlertModal
        console.log(res.error);
        const modalRef = this.modalService.open(AlertLagreModal, {
          backdrop: 'static',
          keyboard: false,
        });
        let textBody: string = res.error;
        modalRef.componentInstance.updateBody(textBody);
      }
    );
  }

  hentAlleBestillinger() {
    this._http.get<Bestilling[]>('/api/admin/bestillinger').subscribe(
      (bestillinger) => {
        this.alleBestillinger = bestillinger;
      },
      (error) => console.log(error)
    );
  }

  hentAlleFerderForBestilling() {
    this._http
      .get<FerdRute[]>(
        '/api/admin/bestilling/' + this.valgtBestilling.id + '/ferder'
      )
      .subscribe(
        (ferder) => {
          this.alleFerder = ferder;
        },
        (error) => console.log(error)
      );
  }
}
