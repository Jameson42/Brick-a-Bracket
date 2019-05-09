import {Category} from './category';

export class Tournament {
    Categories: Array<Category>;
    MocIds: Array<number>;
    TournamentType: string;
    Name: string;
    _id: number;
}
export interface TournamentSummary {
    TournamentType: string;
    Name: string;
    _id: number;
}
export interface TournamentMetadata {
    Tournament: Tournament;
    CategoryIndex: number;
    RoundIndex: number;
    MatchIndex: number;
}
