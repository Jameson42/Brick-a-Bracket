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
    standing: Standing;
    scores: Array<Score>;
}
