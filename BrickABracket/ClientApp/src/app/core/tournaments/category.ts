import {Round} from './round';
import {Standing} from './standing';

export interface Category {
    name: string;
    classificationId: number;
    rounds: Array<Round>;
    mocIds: Array<number>;
    standings: Array<Standing>;
}
