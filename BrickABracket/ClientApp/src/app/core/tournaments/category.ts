import {Round} from './round';
import {Standing} from './standing';

export interface Category {
    Name: string;
    ClassificationId: number;
    Rounds: Array<Round>;
    MocIds: Array<number>;
    Standings: Array<Standing>;
}
