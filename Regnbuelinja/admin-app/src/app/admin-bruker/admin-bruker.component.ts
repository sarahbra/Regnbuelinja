import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { NavbarService } from '../nav-meny/nav-meny.service';
import { AdminPersonalia } from '../models/adminPersonalia';

@Component({
  templateUrl: './admin-bruker.component.html',
})
export class AdminBrukerComponent implements OnInit {
  adminBrukerPersonalia: AdminPersonalia;
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
    this.hentAdminBrukerPersonalia();
  }

  hentAdminBrukerPersonalia() {
    this._http.get<AdminPersonalia>('/api/admin/profil').subscribe(
      (adminBrukerPersonalia) => {
        this.adminBrukerPersonalia = adminBrukerPersonalia;
        this.laster = false;
      },
      (error) => console.log(error)
    );
  }

  visModalOgEndrePassord() {}
}
