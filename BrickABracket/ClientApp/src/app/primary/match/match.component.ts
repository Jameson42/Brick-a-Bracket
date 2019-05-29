import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { TournamentMetadata, Match, Category,
  TournamentService, Score, RedirectService } from '@bab/core';

@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.css']
})
export class MatchComponent implements OnInit {

  private tournamentData$: Observable<TournamentMetadata>;
  private category$: Observable<Category>;
  private match$: Observable<Match>;

  constructor(
    private tournaments: TournamentService,
    private redirect: RedirectService,
  ) { }

  ngOnInit() {
    this.redirect.start('primary');
    this.tournamentData$ = this.tournaments.metadata;
    this.category$ = this.tournaments.category;
    this.match$ = this.tournaments.match;
  }

  score$(player: number): Observable<Score> {
    return this.match$.pipe(
      map(m => {
        if (!m || !m.results || m.results.length <= 0 || player <  0) {
          return null;
        }
        const result = m.results[m.results.length - 1];
        if (!result.scores || result.scores.length <= 0) {
          return null;
        }
        const filteredScores = result.scores.filter(s => s.player === player);
        if (!filteredScores || filteredScores.length <= 0) {
          return null;
        }
        return filteredScores[0];
      }),
    );
  }

}
