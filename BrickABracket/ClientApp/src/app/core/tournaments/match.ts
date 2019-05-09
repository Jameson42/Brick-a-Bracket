export interface Match {
    MocIds: Array<number>;
    Results: Array<MatchResult>;
}

export interface MatchResult {
    Scores: Array<Score>;
}

export interface Score {
    Player: number;
    Time: number;
}
