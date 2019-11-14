export interface Match {
    mocIds: Array<number>;
    results: Array<MatchResult>;
}

export interface MatchResult {
    scores: Array<Score>;
}

export interface Score {
    player: number;
    time: number;
}
