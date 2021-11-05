import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: 'alert-avhengigheter-finnes.modal.html',
})
export class AlertAvhengigheterFinnesModal {
  body: string;
  updateBody(input: string) {
    this.body = input;
  }

  constructor(public modal: NgbActiveModal) {}
}
