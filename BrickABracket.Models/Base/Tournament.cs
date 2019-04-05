﻿using System.Collections.Generic;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models.Base
{
    public class Tournament : IDBItem, ITournamentSummary
    {
        public List<Category> Categories {get;} = new List<Category>();
        public List<int> MocIds {get;} = new List<int>();
        public string TournamentType {get;set;}
        public string Name {get;set;}
        public int _id { get; set; }
    }
}