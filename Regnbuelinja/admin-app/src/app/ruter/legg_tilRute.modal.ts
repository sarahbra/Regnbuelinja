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
import { Rute } from '../models/rute';

@Component({
  templateUrl: 'legg_tilRute.modal.html',
})
export class LeggTilRuteModal {
  forms: FormGroup;
  fb: FormBuilder = new FormBuilder();

  allForms = {
    startpunktForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZøæåØÆÅ\\-. ]{2,30}'),
      ]),
    ],
    endepunktForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZøæåØÆÅ\\-. ]{2,30}'),
      ]),
    ],
    prisForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('^-?[0-9]\\d*(\\.\\d*)?$'),
      ]),
    ],
  };

  constructor(public modal: NgbActiveModal, private http: HttpClient) {}

  ngOnInit() {
    this.forms = this.fb.group(this.allForms);
  }

  vedSubmit() {
    const startpunkt = this.forms.value.startpunktForm;
    const endepunkt = this.forms.value.endepunktForm;
    const pris = parseFloat(this.forms.value.prisForm); //
    const rute = new Rute(startpunkt, endepunkt, pris);
    //const rute = null;

    this.http.post('/api/admin/ruter', rute).subscribe(
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
