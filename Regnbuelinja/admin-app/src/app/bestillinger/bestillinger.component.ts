import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Bestilling } from '../models/bestilling';
import { NavbarService } from '../nav-meny/nav-meny.service';
import { LeggTilBestillingModal } from './legg_tilBestilling.modal';
import { BekreftSlettModal } from '../modals/slett-modaler/bekreft-slett.modal';
import { SlettErrorModal } from '../modals/slett-modaler/slett-error.modal';
import { VisBilletterForBestilling } from '../modals/vis-billetter-for-bestilling.modal';

@Component({
  templateUrl: './bestillinger.component.html',
})
export class BestillingerComponent implements OnInit {
  alleBestillinger: Array<Bestilling> = [];
  laster: boolean = false;

  constructor(
    private _http: HttpClient,
    private _router: Router,
    private modalService: NgbModal,
    public nav: NavbarService
  ) {}

  ngOnInit() {
    this.laster = true;
    this.hentAlleBestillinger();
    this.nav.show();
  }

  hentAlleBestillinger() {
    this._http.get<Bestilling[]>('/api/admin/bestillinger').subscribe(
      (bestillinger) => {
        this.alleBestillinger = bestillinger;
        this.laster = false;
        //Dersom bestillingen er betalt kan ikke bestilling endres -> Endre knapp skjules
      },
      (error) => console.log(error)
    );
  }

  endreBestilling(id: number) {}

  visModalOgSlett(id: number) {
    const modalRef = this.modalService.open(BekreftSlettModal, {
      backdrop: 'static',
      keyboard: false,
    });
    let textBody: string = 'Vil du slette bestilling med id ' + id + '?';
    modalRef.componentInstance.updateBody(textBody);

    modalRef.result.then((retur) => {
      if (retur == 'Slett') {
        this._http.delete('/api/admin/bestilling/' + id).subscribe(
          () => {
            this.hentAlleBestillinger();
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
      this._router.navigate(['/bestillinger']);
    });
  }

  visBilletter(id: number) {
    const modalRef = this.modalService.open(VisBilletterForBestilling, {
      backdrop: 'static',
      keyboard: false,
      size: 'lg',
    });
    let title: string = 'Billetter for bestilling';
    modalRef.componentInstance.updateBody(title);
    //Bestillings-id sendes som input til modal:
    (<VisBilletterForBestilling>modalRef.componentInstance).idAsInput = id;
  }

  leggTilBestilling() {
    const modalRef = this.modalService.open(LeggTilBestillingModal, {
      backdrop: 'static',
      keyboard: false,
    });

    modalRef.result.then((retur) => {
      if (retur == 'Vellykket') this.hentAlleBestillinger();
    });
  }
}
