export class Rute {
  id: number;
  startpunkt: string;
  endepunkt: string;
  pris: number;

  constructor(startpunkt: string, endepunkt: string, pris: number) {
    this.startpunkt = startpunkt;
    this.endepunkt = endepunkt;
    this.pris = pris;
  }
}
