import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { Kunde } from '../models/kunde';
import { NavbarService } from '../nav-meny/nav-meny.service';
import { LeggTilKundeModal } from './legg_tilKunde.modal';
import { BekreftSlettModal } from '../modals/slett-modaler/bekreft-slett.modal';
import { AlertAvhengigheterFinnesModal } from '../modals/slett-modaler/alert-avhengigheter-finnes.modal';
import { VisAvhengigheterModal } from '../modals/slett-modaler/vis-avhengigheter.modal';

@Component({
  templateUrl: './kunde.component.html',
})
export class KundeComponent implements OnInit {
  alleKunder: Array<Kunde> = [];
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
    this.hentAlleKunder();
  }

  hentAlleKunder() {
    this._http.get<Kunde[]>('/api/admin/kunder').subscribe(
      (kundene) => {
        this.alleKunder = kundene;
        this.laster = false;
      },
      (error) => console.log(error)
    );
  }

  leggTilKunde() {
    const modalRef = this.modalService.open(LeggTilKundeModal, {
      backdrop: 'static', 
      keyboard: false,
      size: 'lg'
    });

    modalRef.result.then((retur) => {
      if (retur == "Vellykket")
      this.hentAlleKunder();
    });
  
   }

  visModalOgSlett(id: number) {
    const modalRef = this.modalService.open(BekreftSlettModal, {
      backdrop: 'static',
      keyboard: false,
    });

    let textBody: string = 'Vil du slette kunde med id ' + id + '?';
    modalRef.componentInstance.updateBody(textBody);

    modalRef.result.then((retur) => {
      if (retur == 'Slett') {
        this._http.delete('/api/admin/kunde/' + id).subscribe(
          () => {
            this.hentAlleKunder();
          },
          (res) => {
            const modalRef = this.modalService.open(
              AlertAvhengigheterFinnesModal,
              {
                backdrop: 'static',
                keyboard: false,
              }
            );
            let textBody: string = res.error;
            modalRef.componentInstance.updateBody(textBody);
            //Modal for å vise billetter knyttet til ferd hvis bruker klikker "Vis billetter"
            modalRef.result.then((retur) => {
              if (retur == 'Vis') {
                const modalRef = this.modalService.open(VisAvhengigheterModal, {
                  backdrop: 'static',
                  keyboard: false,
                  size: 'lg',
                });
                let textBody: string =
                  'Bestillinger tilknyttet kunde med id ' +
                  id +
                  '\nmå slettes før kunden kan slettes';
                modalRef.componentInstance.updateBody(textBody);
                (<VisAvhengigheterModal>modalRef.componentInstance).idAsInput =
                  id;
                (<VisAvhengigheterModal>(
                  modalRef.componentInstance
                )).endepunktAsInput = 'kunde';
              }
            });
          }
        );
      }
      this._router.navigate(['/kunder']);
    });
  }

  endreKunde(id: number) {}
}
