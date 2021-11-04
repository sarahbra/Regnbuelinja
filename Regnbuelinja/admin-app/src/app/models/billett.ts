export class Billett {
    id: number;
    fId: number;
    bId: number;
    voksen: boolean;

    constructor(fId: number, bId: number, voksen: boolean){
        this.fId = fId;
        this.bId = bId;
        this.voksen = voksen;
    }
}
