export class Bestilling {
  id: number;
  kId: number;
  totalpris: number;
  betalt: boolean;

  constructor(kId: number, totalpris: number, betalt: boolean){
    this.kId = kId;
    this.totalpris = totalpris;
    this.betalt = betalt;
  }
}
