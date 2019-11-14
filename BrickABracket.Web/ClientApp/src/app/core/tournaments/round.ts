import {Match} from './match';
import {Standing} from './standing';

export interface Round {
    matches: Array<Match>;
    standings: Array<Standing>;
    mocIds: Array<number>;
}
