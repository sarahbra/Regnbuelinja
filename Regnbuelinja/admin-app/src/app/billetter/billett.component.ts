import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { Billett } from '../models/billett';
import { NavbarService } from '../nav-meny/nav-meny.service';
import { LeggTilBillettModal } from './legg_tilBillett.modal';
import { BekreftSlettModal } from '../modals/slett-modaler/bekreft-slett.modal';
import { SlettErrorModal } from '../modals/slett-modaler/slett-error.modal';

@Component({
  templateUrl: './billett.component.html',
})
export class BillettComponent implements OnInit {
  alleBilletter: Array<Billett> = [];
  laster: boolean;

  constructor(
    private _http: HttpClient,
    private _router: Router,
    private modalService: NgbModal,
    public nav: NavbarService
  ) {}

  ngOnInit() {
    this.nav.show();
    this.laster = true;
    this.hentAllebilletter();
  }

  hentAllebilletter() {
    this._http.get<Billett[]>('/api/admin/billetter').subscribe(
      (billettene) => {
        this.alleBilletter = billettene;
        this.laster = false;
      },
      (error) => console.log(error)
    );
  }



  leggTilBillett() { 
    const modalRef = this.modalService.open(LeggTilBillettModal, {
      backdrop: 'static', 
      keyboard: false,
      size: 'lg'
    });

    modalRef.result.then((retur) => {
      if (retur == "Vellykket")
      this.hentAllebilletter();
    });
  
  }

  visModalOgSlett(id: number) {
    const modalRef = this.modalService.open(BekreftSlettModal, {
      backdrop: 'static',
      keyboard: false,
    });
    let textBody: string = 'Vil du slette billett med ' + id + '?';
    modalRef.componentInstance.updateBody(textBody);

    modalRef.result.then((retur) => {
      if (retur == 'Slett') {
        this._http.delete('/api/admin/billett/' + id).subscribe(
          () => {
            this.hentAllebilletter();
          },
          (res) => {
            const modalRef = this.modalService.open(SlettErrorModal, {
              backdrop: 'static',
              keyboard: false,
            });
            let textBody: string = res.error;
            modalRef.componentInstance.updateBody(textBody);
          }
        );
      }
      this._router.navigate(['/billetter']);
    });
  }
}
