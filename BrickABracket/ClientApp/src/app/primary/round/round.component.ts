import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { TournamentMetadata, TournamentService, 
  Category, Round, RedirectService } from '@bab/core';

@Component({
  selector: 'app-round',
  templateUrl: './round.component.html',
  styleUrls: ['./round.component.css']
})
export class RoundComponent implements OnInit {

  private tournamentData$: Observable<TournamentMetadata>;
  private category$: Observable<Category>;
  private round$: Observable<Round>;

  constructor(
    private tournaments: TournamentService,
    private redirect: RedirectService,
  ) { }

  ngOnInit() {
    this.tournamentData$ = this.tournaments.metadata;
    this.category$ = this.tournaments.category;
    this.round$ = this.tournaments.round;
  }

}
