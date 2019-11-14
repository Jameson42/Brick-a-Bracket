import {Category} from './category';

export class Tournament {
    categories: Array<Category>;
    mocIds: Array<number>;
    tournamentType: string;
    name: string;
    _id: number;
}
export interface TournamentSummary {
    tournamentType: string;
    name: string;
    _id: number;
}
export interface TournamentMetadata {
    tournament: Tournament;
    categoryIndex: number;
    roundIndex: number;
    matchIndex: number;
}
