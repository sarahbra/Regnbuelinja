export class AdminPersonalia {
  id: number;
  fornavn: string;
  etternavn: string;
  epost: string;
  telefonnr: string;

  constructor(
    id: number,
    fornavn: string,
    etternavn: string,
    epost: string,
    telefonnr: string
  ) {
    this.id = id;
    this.fornavn = fornavn;
    this.etternavn = etternavn;
    this.epost = epost;
    this.telefonnr = telefonnr;
  }
}
