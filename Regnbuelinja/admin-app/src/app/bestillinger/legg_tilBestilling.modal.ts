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
import { Bestilling } from '../models/bestilling';

@Component({
  templateUrl: './legg_tilBestilling.modal.html',
})
export class LeggTilBestillingModal {
  forms: FormGroup;
  fb: FormBuilder = new FormBuilder();
  betalt: any;
  alleKunder: Array<Kunde> = [];

    allForms = {
      kIdForm: [null, Validators.compose([Validators.required, Validators.pattern(/^\d+$/)])]
    }

  constructor(public modal: NgbActiveModal, private _http: HttpClient) {}

    ngOnInit() {
        this.forms = this.fb.group(this.allForms);
        this.betalt = document.getElementById("betalt");
        this.betalt.checked = false;
        this.hentAlleKunder();
    }
  
    vedSubmit() {
      const kId = this.forms.value.kIdForm;
      
      const bestilling = new Bestilling(kId, 0, this.betalt.checked);
      
      this._http.post('/api/admin/bestillinger', bestilling).subscribe(
        (ok) => {
          if (ok)
           this.modal.close("Vellykket");
          else{
            this.modal.close("Mislykket");
          }
        },
        (error) => {
          this.modal.close("Mislykket");
          console.log(error)
        }
      );
    }

    hentAlleKunder() {
      this._http.get<Kunde[]>('/api/admin/kunder').subscribe(
        (rutene) => {
          this.alleKunder = rutene;
        },
        (error) => console.log(error)
      );
    }
}
