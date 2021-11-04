import { Component, Inject } from '@angular/core';
import { APP_BASE_HREF } from '@angular/common';
import { NavbarService } from './nav-meny.service';

@Component({
  //Denne skal erstatte app-navn-meny i app.component.html
  selector: 'app-nav-meny',
  templateUrl: './nav-meny.component.html',
  styleUrls: ['./nav-meny.css'],
})
export class NavMenyComponent {
  constructor(
    public nav: NavbarService,
    @Inject(APP_BASE_HREF) baseHref: string
  ) {
    this.logoutUrl = `/api/admin/logg_ut?returUrl=${encodeURIComponent(
      baseHref + 'login'
    )}`;
  }

  isExpanded = false;

  logoutUrl = '';

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
