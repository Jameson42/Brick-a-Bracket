import {Match} from './match';
import {Standing} from './standing';

export interface Round {
    Matches: Array<Match>;
    Standings: Array<Standing>;
}
