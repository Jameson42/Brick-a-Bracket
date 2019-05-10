import { Component, OnInit } from '@angular/core';
import { TournamentService } from 'src/app/core/tournaments/tournament.service';
import { Observable } from 'rxjs';
import { TournamentSummary } from 'src/app/core/tournaments/tournament';

@Component({
  selector: 'app-tournament-list',
  templateUrl: './tournament-list.component.html',
  styleUrls: ['./tournament-list.component.css']
})
export class TournamentListComponent implements OnInit {

  private summaries$: Observable<Array<TournamentSummary>>;

  constructor(private tournaments: TournamentService) { }

  ngOnInit() {
    this.summaries$ = this.tournaments.summaries;
  }

}
