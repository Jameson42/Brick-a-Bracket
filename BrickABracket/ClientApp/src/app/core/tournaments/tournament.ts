class Tournament {
    public Categories: Category[];
    public MocIds: number[];
    public TournamentType: string;
    public Name: string;
    public _id: number;
}
class TournamentSummary {
    public TournamentType: string;
    public Name: string;
    public _id: number;
}
class TournamentMetadata {
    public Tournament: Tournament;
    public CategoryIndex: number;
    public RoundIndex: number;
    public MatchIndex: number;
}