import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: 'alert-dato.modal.html',
})
export class AlertDatoModal {
  body: string;
  updateBody(input: string) {
    this.body = input;
  }

  constructor(public modal: NgbActiveModal) {}
}
