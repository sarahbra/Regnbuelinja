import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Bruker } from '../models/bruker';
import { NavbarService } from '../nav-meny/nav-meny.service';

@Component({
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  loginSkjema: FormGroup;
  loginError: boolean;


  validering = {
    brukernavn: [null, Validators.compose([Validators.required, Validators.pattern('[a-zA-ZøæåØÆÅ\\-. ]{2,30}')])],
    passord: [null, Validators.compose([Validators.required, Validators.pattern('^(?=.*[0-9])(?=.*[a-zA-ZæøåÆØÅ])([a-zA-ZæøåÆØÅ0-9]+){6,}$')])]
  };

  constructor(private http: HttpClient, private fb: FormBuilder, private router: Router, public nav: NavbarService) {
    this.loginSkjema = fb.group(this.validering);

  }

  ngOnInit() {
    this.nav.hide()
    this.loginError = false;
  }


  vedSubmit() {
    const brukernavn = this.loginSkjema.value.brukernavn;
    const passord = this.loginSkjema.value.passord;
    const login = new Bruker(brukernavn, passord);

    this.http.post('/api/admin/logg_inn', login).subscribe(
      (ok) => {
        if (ok) {
          this.router.navigate(['/ruter']);
        } else {
          this.loginError = true;
        }

      },
      (error) => console.log(error)
    );
  }
}
