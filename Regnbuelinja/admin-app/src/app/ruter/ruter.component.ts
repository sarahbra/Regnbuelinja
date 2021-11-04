import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Rute } from '../models/rute';
import { Router } from '@angular/router';
import { BekreftSlettModal } from '../modals/slett-modaler/bekreft-slett.modal';
import { AlertAvhengigheterFinnesModal } from '../modals/slett-modaler/alert-avhengigheter-finnes.modal';
import { NavbarService } from '../nav-meny/nav-meny.service';
import { LeggTilRuteModal } from './legg_tilRute.modal';
import { VisAvhengigheterModal } from '../modals/slett-modaler/vis-avhengigheter.modal';

@Component({
  //selector: 'app-ruter', -> Det er routing som gjelder så denne gjør
  templateUrl: './ruter.component.html',
  //styleUrls: ['./ruter.css'],
})
export class RuterComponent implements OnInit {
  alleRuter: Array<Rute> = [];
  laster: boolean = false;

  constructor(
    private _http: HttpClient,
    private _router: Router,
    private modalService: NgbModal,
    public nav: NavbarService
  ) {}

  ngOnInit() {
    this.nav.show();
    this.laster = true;
    this.hentAlleRuter();
  }

  hentAlleRuter() {
    this._http.get<Rute[]>('/api/admin/ruter').subscribe(
      (rutene) => {
        this.alleRuter = rutene;
        this.laster = false;
      },
      (error) => console.log(error)
    );
  }

  endreRute(id: number) {}

  leggTilRute() {
    const modalRef = this.modalService.open(LeggTilRuteModal, {
      backdrop: 'static',
      keyboard: false,
      size: 'lg'
    });

    modalRef.result.then((retur) => {
      if (retur == 'Vellykket') console.log("I'm getting hit with a broom");
      this.hentAlleRuter();
    });
  }

  visModalOgSlett(id: number) {
    console.log(id);
    const modalRef = this.modalService.open(BekreftSlettModal, {
      backdrop: 'static',
      keyboard: false,
    });

    let textBody: string =
      'Vil du slette rute med id ' +
      id +
      '? Ved sletting vil alle ferder med denne ruten også bli slettet!';
    modalRef.componentInstance.updateBody(textBody);

    modalRef.result.then((retur) => {
      if (retur == 'Slett') {
        this._http.delete('/api/admin/rute/' + id).subscribe(
          () => {
            this.hentAlleRuter();
          },
          //Lage en kulere alert dialog?
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
            //Modal for å vise billetter knyttet til rute hvis bruker klikker "Vis billetter"
            modalRef.result.then((retur) => {
              if (retur == 'Vis') {
                const modalRef = this.modalService.open(VisAvhengigheterModal, {
                  backdrop: 'static',
                  keyboard: false,
                  size: 'lg',
                });
                let textBody: string =
                  'Billetter tilknyttet rute med id ' +
                  id +
                  '\nmå slettes før ruten kan slettes';
                modalRef.componentInstance.updateBody(textBody);
                (<VisAvhengigheterModal>modalRef.componentInstance).idAsInput =
                  id;
                (<VisAvhengigheterModal>(
                  modalRef.componentInstance
                )).endepunktAsInput = 'rute';
              }
            });
          }
        );
      }
      this._router.navigate(['/ruter']);
    });
  }
}
