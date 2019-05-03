class Match {
    public MocIds: number[];
    public Results: MatchResult[];
}

class MatchResult {
    public Scores: Score[];
}

class Score {
    public Player: number;
    public Time: number;
}