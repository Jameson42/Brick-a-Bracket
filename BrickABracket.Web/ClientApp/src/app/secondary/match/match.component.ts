import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Match, TournamentService, RedirectService } from '@bab/core';

@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.css']
})
export class MatchComponent implements OnInit {

  private match$: Observable<Match>;

  constructor(
    private tournaments: TournamentService,
    private redirect: RedirectService,
  ) { }

  ngOnInit() {
    this.redirect.cancel();
    this.match$ = this.tournaments.match;
  }

}
