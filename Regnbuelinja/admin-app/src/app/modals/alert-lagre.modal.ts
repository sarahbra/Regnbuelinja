import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: 'alert-lagre.modal.html',
})
export class AlertLagreModal {
  body: string;
  updateBody(input: string) {
    this.body = input;
  }

  constructor(public modal: NgbActiveModal) {}
}
