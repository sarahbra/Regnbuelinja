export class Ferd {
  fId: number;
  bId: number;
  rId: number;
  avreiseTid: string;
  ankomstTid: string;

  constructor(
    bId: number,
    rId: number,
    avreiseTid: string,
    ankomstTid: string
  ) {
    this.bId = bId;
    this.rId = rId;
    this.avreiseTid = avreiseTid;
    this.ankomstTid = ankomstTid;
  }
}
