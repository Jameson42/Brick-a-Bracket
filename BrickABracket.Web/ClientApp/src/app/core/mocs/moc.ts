import { Standing } from '@bab/core/tournaments/standing';
import { Score } from '../tournaments/match';


export class Moc {
    name: string;
    classificationId: number;
    competitorId: number;
    weight: number;
    _id: number;
}

export class MocDisplay extends Moc {

    constructor(moc: Moc) {
        super();
        this._id = moc._id;
        this.classificationId = moc.classificationId;
        this.competitorId = moc.competitorId;
        this.name = moc.name;
        this.weight = moc.weight;
    }
    standing: Standing;
    scores: Array<Score>;
}

export class MocClassificationGrouping {
    classificationId: number;
    mocs: Array<Moc>;

    constructor(moc: Moc) {
        this.classificationId = moc.classificationId;
        this.mocs = new Array<Moc>(moc);
    }
}
