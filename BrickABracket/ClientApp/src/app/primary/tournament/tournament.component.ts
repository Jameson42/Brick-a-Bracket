import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { TournamentService, MocService, 
  MocClassificationGrouping, Tournament, 
  RedirectService } from '@bab/core';

@Component({
  selector: 'app-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.css']
})
export class TournamentComponent implements OnInit {

  private mocGroupings$: Observable<Array<MocClassificationGrouping>>;
  private tournament$: Observable<Tournament>;

  constructor(
    private tournaments: TournamentService,
    private mocs: MocService,
    private redirect: RedirectService,
  ) { }

  ngOnInit() {
    this.tournament$ = this.tournaments.tournament;
    this.mocGroupings$ = this.mocs.getClassificationGroupings(
      this.tournament$.pipe(map(t => t.mocIds)));
  }

}
