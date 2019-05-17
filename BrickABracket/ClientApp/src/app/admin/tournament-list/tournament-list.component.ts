import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { TournamentService, TournamentSummary } from '@bab/core';

@Component({
  selector: 'app-tournament-list',
  templateUrl: './tournament-list.component.html',
  styleUrls: ['./tournament-list.component.css']
})
export class TournamentListComponent implements OnInit {

  private summaries$: Observable<Array<TournamentSummary>>;

  constructor(
    private tournaments: TournamentService,
    ) { }

  ngOnInit() {
    this.summaries$ = this.tournaments.summaries;
  }

  async delete(id: number) {
    await this.tournaments.delete(id);
  }
}
