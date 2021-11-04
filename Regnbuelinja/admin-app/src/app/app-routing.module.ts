import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RuterComponent } from './ruter/ruter.component';
import { BaaterComponent } from './baater/baater.component';
import { FerderComponent } from './ferder/ferder.component';
import { BestillingerComponent } from './bestillinger/bestillinger.component';
import { KundeComponent } from './kunder/kunde.component';
import { BillettComponent } from './billetter/billett.component';
import { EndreComponent } from './endre/endre.component';
import { AdminBrukerComponent } from './admin-bruker/admin-bruker.component';

const routes: Routes = [
  { path: 'ruter', component: RuterComponent },
  { path: 'ferder', component: FerderComponent },
  { path: 'baater', component: BaaterComponent },
  { path: 'bestillinger', component: BestillingerComponent },
  { path: 'kunder', component: KundeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'billetter', component: BillettComponent },
  { path: 'admin', component: AdminBrukerComponent },
  { path: 'endre/:id/:type', component: EndreComponent },
  { path: '', redirectTo: 'login', pathMatch: 'full' }, //Dette er default: Hvis ingenting er oppgitt
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
