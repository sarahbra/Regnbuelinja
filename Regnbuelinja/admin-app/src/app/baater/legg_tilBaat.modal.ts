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
import { Baat } from '../models/baat';

@Component({
  templateUrl: './legg_tilBaat.modal.html',
})
export class LeggTilBaatModal {
  forms: FormGroup;
  fb: FormBuilder = new FormBuilder();

  allForms = {
    navnForm: [
      null,
      Validators.compose([
        Validators.required,
        Validators.pattern('[a-zA-ZøæåØÆÅ ]{2,30}'),
      ]),
    ],
  };

  constructor(public modal: NgbActiveModal, private http: HttpClient) {}

  ngOnInit() {
    this.forms = this.fb.group(this.allForms);
  }

  vedSubmit() {
    const navn = this.forms.value.navnForm;
    const baat = new Baat(navn);
    //const rute = null;

    this.http.post('/api/admin/baater', baat).subscribe(
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
