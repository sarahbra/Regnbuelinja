import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { NavbarService } from '../nav-meny/nav-meny.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Baat } from '../models/baat';
import { Rute } from '../models/rute';
import { Kunde } from '../models/kunde';
import { Ferd } from '../models/ferd';
import { formatDate } from '@angular/common';
import { Bestilling } from '../models/bestilling';
import { AdminPersonalia } from '../models/adminPersonalia';
import { AdminBrukerPassord } from '../models/adminBrukerPassord';
import { Billett } from '../models/billett';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertAvhengigheterFinnesModal } from '../modals/slett-modaler/alert-avhengigheter-finnes.modal';
import { VisAvhengigheterModal } from '../modals/slett-modaler/vis-avhengigheter.modal';

@Component({
  templateUrl: './endre.component.html',
})
export class EndreComponent implements OnInit {
  skjemaBaat: FormGroup;
  skjemaRute: FormGroup;
  skjemaKunde: FormGroup;
  skjemaFerd: FormGroup;
  skjemaBillett: FormGroup;
  skjemaBestilling: FormGroup;
  skjemaAdmin: FormGroup;
  skjemaAdminPassord: FormGroup;

  visEndreBaat: boolean = false;
  visEndreRute: boolean = false;
  visEndreKunde: boolean = false;
  visEndreFerd: boolean = false;
  visEndreAdmin: boolean = false;
  visEndreBestilling: boolean = false;
  visEndreAdminPassord: boolean = false;

  visEndreBillett: boolean = false;
  alleBaater: Array<Baat> = [];
  alleRuter: Array<Rute> = [];
  alleBestillinger: Array<Bestilling> = [];
  alleKunder: Array<Kunde> = [];
  adminBrukerPassord: AdminBrukerPassord;
  alleBilletter: Array<Billett> = [];

  //Idér for å printe ut på endreSiden
  bestillingsId: number;
  ruteId: number;
  ferdId: number;
  baatId: number;
  kundeId: number;
  billettId: number;

  //Brukernavn admin

  //Formater dato
  dateArray: Array<string> = [];
  datoSplittet: Array<string> = [];
  tidSplittet: Array<string> = [];
  dato: Date;
  isoDato: string = '';

  valideringBaat = {
    id: [null],
    navn: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZæøåÆØÅ ]{2,30}'),
      ]),
    ],
  };

  valideringRute = {
    id: [null],
    startpunkt: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZæøåÆØÅ]{2,30}'),
      ]),
    ],
    endepunkt: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZæøåÆØÅ]{2,30}'),
      ]),
    ],
    pris: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(/^[1-9]\d{0,7}(?:\.\d{1,4})?$/),
      ]),
    ],
  };

  valideringKundeOgAdmin = {
    id: [null],
    fornavn: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZæøåÆØÅ]{2,30}'),
      ]),
    ],
    etternavn: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZæøåÆØÅ]{2,30}'),
      ]),
    ],
    epost: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(
          /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/
        ),
      ]),
    ],
    telefonnr: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('^[0-9]{8}$'),
      ]),
    ],
  };

  valideringFerd = {
    fId: [null],
    bId: [null, Validators.required],
    rId: [null, Validators.required],
    avreiseTid: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(
          /^([1-9]|([012][0-9])|(3[01])).([0]{0,1}[1-9]|1[012]).\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/
        ),
      ]),
    ],
    ankomstTid: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern(
          /^([1-9]|([012][0-9])|(3[01])).([0]{0,1}[1-9]|1[012]).\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/
        ),
      ]),
    ],
  };

  valideringBestilling = {
    id: [null, Validators.required],
    kId: [null, Validators.required],
    totalpris: [null, Validators.required],
    betalt: ['false', Validators.required],
  };

  valideringPassord = {
    //Legge til pattern her!
    passord: [null, Validators.required],
  };
  valideringBillett = {
    id: [null, Validators.required],
    fId: [null, Validators.required],
    bId: [null, Validators.required],
    voksen: [null, Validators.required],
  };

  constructor(
    private _http: HttpClient,
    private _router: Router,
    private _route: ActivatedRoute,
    private _fb: FormBuilder,
    public nav: NavbarService,
    private modalService: NgbModal
  ) {
    this.skjemaBaat = _fb.group(this.valideringBaat);
    this.skjemaRute = _fb.group(this.valideringRute);
    this.skjemaKunde = _fb.group(this.valideringKundeOgAdmin);
    this.skjemaFerd = _fb.group(this.valideringFerd);
    this.skjemaBestilling = _fb.group(this.valideringBestilling);
    this.skjemaAdmin = _fb.group(this.valideringKundeOgAdmin);
    this.skjemaAdminPassord = _fb.group(this.valideringPassord);
    this.skjemaBillett = _fb.group(this.valideringBillett);
  }

  ngOnInit() {
    this.nav.hide();
    this._route.params.subscribe(
      (params) => {
        switch (params.type) {
          case 'baat':
            this.visEndreBaat = true;
            this.hentEnBaat(params.id);
            this.baatId = params.id;
            break;
          case 'rute':
            this.visEndreRute = true;
            this.hentEnRute(params.id);
            this.ruteId = params.id;
            break;
          case 'kunde':
            this.visEndreKunde = true;
            this.hentEnKunde(params.id);
            this.kundeId = params.id;
            break;
          case 'ferd':
            this.visEndreFerd = true;
            this.hentEnFerd(params.id);
            this.ferdId = params.id;
            break;
          case 'bestilling':
            this.visEndreBestilling = true;
            this.hentEnBestilling(params.id);
            this.bestillingsId = params.id;
            break;
          case 'admin':
            this.visEndreAdmin = true;
            this.hentAdminPersonalia();
            break;
          case 'adminPassord':
            this.visEndreAdminPassord = true;
            this.hentAdminBrukernavn();
            break;
          case 'billett':
            this.visEndreBillett = true;
            this.hentEnBillett(params.id);
            this.billettId = params.id;
            break;
          default:
            console.log(params.type);
            break;
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  vedSubmit(type: string) {
    switch (type) {
      case 'baat':
        this.endreBaat();
        break;
      case 'rute':
        this.endreRute();
        break;
      case 'kunde':
        this.endreKunde();
        break;
      case 'ferd':
        this.endreFerd();
        break;
      case 'bestilling':
        this.endreBestilling();
        break;
      case 'admin':
        this.endreAdminPersonalia();
        break;
      case 'adminPassord':
        this.endreAdminPassord();
        break;
      case 'billett':
        this.endreBillett();
        break;
      default:
        console.log(type);
        break;
    }
  }

  //Trenger endepunkt for å hente en bruker (brukernavn bare! ikke passord!)
  hentAdminBrukernavn() {
    this._http.get<AdminBrukerPassord>('/api/admin/??SARA!').subscribe(
      (adminBrukerPassord) => {
        this.adminBrukerPassord.brukernavn = adminBrukerPassord.brukernavn;
      },
      (error) => console.log(error)
    );
  }
  endreAdminPassord() {
    const passord = this.skjemaAdminPassord.value.passord;

    const endretAdminPassord = new AdminBrukerPassord(
      this.adminBrukerPassord.brukernavn,
      passord
    );

    this._http.put('/api/admin/bruker/', endretAdminPassord).subscribe(
      (retur) => {
        this._router.navigate(['/admin']);
      },
      (error) => console.log(error)
    );
  }

  hentAdminPersonalia() {
    this._http.get<AdminPersonalia>('/api/admin/profil').subscribe(
      (adminBrukerPersonalia) => {
        this.skjemaKunde.patchValue({ id: adminBrukerPersonalia.id });
        this.skjemaKunde.patchValue({ fornavn: adminBrukerPersonalia.fornavn });
        this.skjemaKunde.patchValue({
          etternavn: adminBrukerPersonalia.etternavn,
        });
        this.skjemaKunde.patchValue({ epost: adminBrukerPersonalia.epost });
        this.skjemaKunde.patchValue({
          telefonnr: adminBrukerPersonalia.telefonnr,
        });
      },
      (error) => console.log(error)
    );
  }

  //Bruk lagreKunde!
  endreAdminPersonalia() {
    const id = this.skjemaAdmin.value.id;
    const fornavn = this.skjemaAdmin.value.fornavn;
    const etternavn = this.skjemaAdmin.value.etternavn;
    const epost = this.skjemaAdmin.value.epost;
    const telefonnr = this.skjemaAdmin.value.telefonnr;

    const endretAdminPersonalia = new AdminPersonalia(
      id,
      fornavn,
      etternavn,
      epost,
      telefonnr
    );

    this._http.put('/api/admin/???? SARA/', endretAdminPersonalia).subscribe(
      (retur) => {
        this._router.navigate(['/admin']);
      },
      (error) => console.log(error)
    );
  }
  hentEnBillett(id: number) {
    this._http.get<Billett>('/api/admin/billett/' + id).subscribe(
      (billett) => {
        this.skjemaBillett.patchValue({ id: billett.id });
        this.skjemaBillett.patchValue({ fId: billett.fId });
        this.skjemaBillett.patchValue({ bId: billett.bId });
        this.skjemaBillett.patchValue({ voksen: billett.voksen.toString() });
      },
      (error) => {
        console.log(error);
      }
    );
  }

  endreBillett() {
    const id = this.skjemaBillett.value.id;
    const fId = this.skjemaBillett.value.fId;
    const bId = this.skjemaBillett.value.bId;
    const voksen = this.skjemaBillett.value.voksen == 'Voksen' ? true : false;
    const endretBillett = new Billett(fId, bId, voksen);

    this._http.put('api/admin/billett/' + id, endretBillett).subscribe(
      (retur) => {
        this._router.navigate(['/billetter']);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  hentEnBaat(id: number) {
    this._http.get<Baat>('/api/admin/baat/' + id).subscribe(
      (baat) => {
        this.skjemaBaat.patchValue({ id: baat.id });
        this.skjemaBaat.patchValue({ navn: baat.navn });
      },
      (error) => {
        console.error(error);
      }
    );
  }

  endreBaat() {
    const id = this.skjemaBaat.value.id;
    const navn = this.skjemaBaat.value.navn;
    const endretBaat = new Baat(navn);
    endretBaat.id = id;
    this._http.put('/api/admin/baat/' + id, endretBaat).subscribe(
      (retur) => {
        this._router.navigate(['/baater']);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  hentEnRute(id: number) {
    this._http.get<Rute>('/api/admin/rute/' + id).subscribe((rute) => {
      this.skjemaRute.patchValue({ id: rute.id });
      this.skjemaRute.patchValue({ startpunkt: rute.startpunkt });
      this.skjemaRute.patchValue({ endepunkt: rute.endepunkt });
      this.skjemaRute.patchValue({ pris: rute.pris });
    });
  }

  endreRute() {
    const id = this.skjemaRute.value.id;
    const startpunkt = this.skjemaRute.value.startpunkt;
    const endepunkt = this.skjemaRute.value.endepunkt;
    const pris = parseFloat(this.skjemaRute.value.pris);

    const endretRute = new Rute(startpunkt, endepunkt, pris);
    endretRute.id = id;
    this._http.put('/api/admin/rute/' + id, endretRute).subscribe(
      (retur) => {
        this._router.navigate(['/ruter']);
      },
      (res) => {
        const modalRef = this.modalService.open(AlertAvhengigheterFinnesModal, {
          backdrop: 'static',
          keyboard: false,
        });
        let textBody: string = res.error;
        modalRef.componentInstance.updateBody(textBody);
        //Modal for å vise billetter knyttet til rute hvis bruker klikker "Vis billetter"
        modalRef.result.then((retur) => {
          if (retur == 'Vis') {
            const modalRef = this.modalService.open(VisAvhengigheterModal, {
              backdrop: 'static',
              keyboard: false,
              size: 'lg',
            });
            let textBody: string =
              'Billetter tilknyttet rute med id ' +
              id +
              '\nmå slettes før ruten kan endres';
            modalRef.componentInstance.updateBody(textBody);
            (<VisAvhengigheterModal>modalRef.componentInstance).idAsInput = id;
            (<VisAvhengigheterModal>(
              modalRef.componentInstance
            )).endepunktAsInput = 'rute';
          }
        });
      }
    );
  }

  hentEnKunde(id: number) {
    this._http.get<Kunde>('/api/admin/kunde/' + id).subscribe(
      (kunde) => {
        this.skjemaKunde.patchValue({ id: kunde.id });
        this.skjemaKunde.patchValue({ fornavn: kunde.fornavn });
        this.skjemaKunde.patchValue({ etternavn: kunde.etternavn });
        this.skjemaKunde.patchValue({ epost: kunde.epost });
        this.skjemaKunde.patchValue({ telefonnr: kunde.telefonnr });
      },
      (error) => console.log(error)
    );
  }

  endreKunde() {
    const id = this.skjemaKunde.value.id;
    const fornavn = this.skjemaKunde.value.fornavn;
    const etternavn = this.skjemaKunde.value.etternavn;
    const epost = this.skjemaKunde.value.epost;
    const telefonnr = this.skjemaKunde.value.telefonnr;

    const endretKunde = new Kunde(fornavn, etternavn, epost, telefonnr);
    endretKunde.id = id;

    this._http.put('/api/admin/kunde/' + id, endretKunde).subscribe(
      (retur) => {
        this._router.navigate(['/kunder']);
      },
      (error) => console.log(error)
    );
  }

  hentEnFerd(fId: number) {
    this.hentAlleBaater();
    this.hentAlleRuter();

    this._http.get<Ferd>('/api/admin/ferd/' + fId).subscribe(
      (ferd) => {
        this.skjemaFerd.patchValue({ fId: ferd.fId });
        this.skjemaFerd.patchValue({ bId: ferd.bId });
        this.skjemaFerd.patchValue({ rId: ferd.rId });
        this.skjemaFerd.patchValue({
          avreiseTid: formatDate(ferd.avreiseTid, 'dd/MM/yyyy HH:mm', 'en-US'),
        });
        this.skjemaFerd.patchValue({
          ankomstTid: formatDate(ferd.ankomstTid, 'dd/MM/yyyy HH:mm', 'en-US'),
        });
      },
      (error) => console.log(error)
    );
  }

  endreFerd() {
    const fId = this.skjemaFerd.value.fId;
    const bId = this.skjemaFerd.value.bId;
    const rId = this.skjemaFerd.value.rId;
    const avreiseTid = this.formaterDato(this.skjemaFerd.value.avreiseTid);
    const ankomstTid = this.formaterDato(this.skjemaFerd.value.ankomstTid);

    const endretFerd = new Ferd(bId, rId, avreiseTid, ankomstTid);
    endretFerd.fId = fId;

    this._http.put('/api/admin/ferd/' + fId, endretFerd).subscribe(
      (retur) => {
        this._router.navigate(['/ferder']);
      },
      (res) => {
        const modalRef = this.modalService.open(AlertAvhengigheterFinnesModal, {
          backdrop: 'static',
          keyboard: false,
        });
        let textBody: string = res.error;
        modalRef.componentInstance.updateBody(textBody);
        //Modal for å vise billetter knyttet til rute hvis bruker klikker "Vis billetter"
        modalRef.result.then((retur) => {
          if (retur == 'Vis') {
            const modalRef = this.modalService.open(VisAvhengigheterModal, {
              backdrop: 'static',
              keyboard: false,
              size: 'lg',
            });
            let textBody: string =
              'Billetter tilknyttet ferd med id ' +
              fId +
              '\nmå slettes før ferden kan endres';
            modalRef.componentInstance.updateBody(textBody);
            (<VisAvhengigheterModal>modalRef.componentInstance).idAsInput = fId;
            (<VisAvhengigheterModal>(
              modalRef.componentInstance
            )).endepunktAsInput = 'ferd';
          }
        });
      }
    );
  }

  hentEnBestilling(id: number) {
    this.hentAlleKunder();

    this._http.get<Bestilling>('/api/admin/bestilling/' + id).subscribe(
      (bestilling) => {
        this.skjemaBestilling.patchValue({ id: bestilling.id });
        this.skjemaBestilling.patchValue({ kId: bestilling.kId });
        this.skjemaBestilling.patchValue({ totalpris: bestilling.totalpris });
        this.skjemaBestilling.patchValue({
          betalt: bestilling.betalt.toString(),
        });
      },
      (error) => console.log(error)
    );
  }

  endreBestilling() {
    const id = this.skjemaBestilling.value.id;
    const kId = this.skjemaBestilling.value.kId;
    const totalpris = this.skjemaBestilling.value.totalpris;
    const betalt = this.skjemaBestilling.value.betalt == 'false' ? false : true;

    const endretBestilling = new Bestilling(kId, totalpris, betalt);
    endretBestilling.id = id;

    this._http.put('/api/admin/bestilling/' + id, endretBestilling).subscribe(
      (retur) => {
        this._router.navigate(['/bestillinger']);
      },
      (error) => console.log(error)
    );
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

  hentAlleKunder() {
    this._http.get<Kunde[]>('/api/admin/kunder').subscribe(
      (kunder) => {
        this.alleKunder = kunder;
      },
      (error) => console.log(error)
    );
  }
}
