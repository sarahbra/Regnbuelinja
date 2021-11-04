import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Billett } from 'src/app/models/billett';
import { HttpClient } from '@angular/common/http';
import { NavigationEnd, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: 'vis-billetter-for-bestilling.modal.html',
})
export class VisBilletterForBestilling implements OnInit {
  title: string;
  updateBody(input: string) {
    this.title = input;
  }
  alleBilletter: Array<Billett> = [];
  laster: boolean;
  //bestillings-id som input
  @Input() idAsInput: number;

  constructor(
    private _http: HttpClient,
    private _router: Router,
    public modal: NgbActiveModal,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    this.laster = true;
    this.hentAlleBilletter();
    //Ved navigering til /billetter lukkes modalen
    this._router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        // lukk modal
        this.modalService.dismissAll();
      }
    });
  }

  hentAlleBilletter() {
    this._http
      .get<Billett[]>('/api/admin/bestilling/' + this.idAsInput + '/billetter')
      .subscribe(
        //NB! må sjekke om listen er tom. Hvis tom så må vi skrive ut at det ikke finnes billetter og bestilling må slettes
        (billetter) => {
          this.alleBilletter = billetter;
          this.laster = false;
        },
        (res) => {
          //Dersom det ikke finnes billetter vises tekst fra server i tittelen på denne modalen
          let title: string = res.error;
          this.title = title;
        }
      );
  }
}
