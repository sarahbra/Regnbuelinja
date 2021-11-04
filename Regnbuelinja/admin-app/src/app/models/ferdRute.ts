//En klasse for å vise eventuell mulig returferder for en bestilling med enveisbillett
//Det er i dette tilfellet mulig å legge til returbillett og da må vi hente mulige returferder basert på avreiseferd sin dato og klokkeslett
// Det er altså for å forhindre at kunden kan bestille returreise mens kunden er midt på havet på en annen reise
export class FerdRute {
  fId: number;
  avreiseTid: string;
  ankomstTid: string;
  strekning: string;

  constructor(
    fId: number,
    avreiseTid: string,
    ankomstTid: string,
    strekning: string
  ) {
    this.fId = fId;
    this.avreiseTid = avreiseTid;
    this.ankomstTid = ankomstTid;
    this.strekning = strekning;
  }
}
