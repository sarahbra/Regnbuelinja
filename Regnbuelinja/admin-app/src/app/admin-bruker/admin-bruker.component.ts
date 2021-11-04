import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NavbarService } from '../nav-meny/nav-meny.service';
import { AdminPersonalia } from '../models/adminPersonalia';

@Component({
  templateUrl: './admin-bruker.component.html',
})
export class AdminBrukerComponent implements OnInit {
  adminBrukerPersonalia: AdminPersonalia;
  lasteStatus: 'ikke-lastet' | 'laster' | 'lastet' = 'ikke-lastet';

  constructor(private _http: HttpClient, public nav: NavbarService) {}

  ngOnInit() {
    this.nav.show();
    this.lasteStatus = 'laster';
    this.hentAdminBrukerPersonalia();
  }

  hentAdminBrukerPersonalia() {
    this._http.get<AdminPersonalia>('/api/admin/profil').subscribe(
      (adminBrukerPersonalia) => {
        this.adminBrukerPersonalia = adminBrukerPersonalia;
        this.lasteStatus = 'lastet';
      },
      (error) => console.log(error)
    );
  }

  visModalOgEndrePassord() {}
}
