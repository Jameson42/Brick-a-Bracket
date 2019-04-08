using LiteDB;
using BrickABracket.Models.Base;

namespace BrickABracket.Core.Services
{
    public class ActivesService
    {
        private int tournamentId;
        private int categoryId;
        private int roundId;
        public int TournamentId 
        {
            get => tournamentId;
            set
            {
                tournamentId = value;
                CategoryId = -1;
            }
        }
        public int CategoryId
        {
            get => categoryId;
            set 
            {
                categoryId = value;
                RoundId = -1;
            }
        }
        public int RoundId
        {
            get => roundId;
            set
            {
                roundId = value;
                MatchId = -1;
            }
        }
        public int MatchId {get;set;}
    }
}