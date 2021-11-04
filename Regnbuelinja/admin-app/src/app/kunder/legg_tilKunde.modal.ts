import { Component, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
  Form,
} from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Kunde } from '../models/kunde';

@Component({
  templateUrl: './legg_tilKunde.modal.html',
})
export class LeggTilKundeModal {
  forms: FormGroup;
  fb: FormBuilder = new FormBuilder();

    allForms = {
      fornavnForm: [null, Validators.compose([Validators.required, Validators.pattern('[a-zA-ZøæåØÆÅ]{2,30}')])],
      etternavnForm: [null, Validators.compose([Validators.required, Validators.pattern('[a-zA-ZøæåØÆÅ]{2,30}')])],
      epostForm: [null, Validators.compose([Validators.required, Validators.pattern(/^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/)])],
      telefonForm: [null, Validators.compose([Validators.required, Validators.pattern('^[0-9]{8}$')])]  // '^-?[0-9]\\d*(\\.\\d*)?$'
    }
    
    constructor(public modal: NgbActiveModal, private http: HttpClient){ }

  ngOnInit() {
    this.forms = this.fb.group(this.allForms);
  }

  vedSubmit() {
    const fornavn = this.forms.value.fornavnForm;
    const etternavn = this.forms.value.etternavnForm;
    const epost = this.forms.value.epostForm;
    const telefon = this.forms.value.telefonForm;

    const kunde = new Kunde(fornavn, etternavn, epost, telefon);
    //const rute = null;

    this.http.post('/api/admin/kunder', kunde).subscribe(
      (ok) => {
        if (ok) {
          this.modal.close('Vellykket');
        } else {
          this.modal.close('Mislykket');
        }
      },
      (error) => {
        this.modal.close('Mislykket');
        console.log(error);
      }
    );
  }
}
