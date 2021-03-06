import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap, shareReplay, tap, map, filter, take, debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { TournamentService, Category, Match, MatchResult } from '@bab/core';

@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.css']
})
export class MatchComponent implements OnInit {

  private category$: Observable<Category>;
  private match$: Observable<Match>;
  private mocIds$: Observable<Array<number>>;
  private results$: Observable<Array<MatchResult>>;
  private roundId$: Observable<number>;
  private id: number;
  private readyColor = 'btn-outline-success';

  constructor(
    private tournaments: TournamentService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit() {
    this.category$ = this.tournaments.category;
    this.roundId$ = this.tournaments.metadata.pipe(
      filter(md => !!md),
      map(md => md.roundIndex),
      distinctUntilChanged(),
      shareReplay(1)
    );
    this.match$ = this.route.paramMap.pipe(
      debounceTime(200),
      distinctUntilChanged((x, y) => x.get('id') === y.get('id')),
      tap(params => {
        this.id = Number(params.get('id'));
        this.tournaments.setMatch(this.id);
      }),
      switchMap(_ => this.tournaments.match),
      shareReplay(1)
    );
    this.mocIds$ = this.match$.pipe(
      filter(m => !!m),
      map(m => m.mocIds),
    );
    this.results$ = this.match$.pipe(
      filter(m => !!m),
      map(m => m.results),
    );
  }

  nextMatch() {
    this.tournaments.nextMatch().then(_ => {
      this.tournaments.metadata.pipe(take(1)).subscribe(md => {
        if (md.matchIndex > -1) {
          this.router.navigate(['../' + md.matchIndex], { relativeTo: this.route });
        } else {
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      });
    });
  }

  delete() {
    this.tournaments.deleteCurrentMatch().then(_ => {
      this.router.navigate(['../'], {relativeTo: this.route});
    });
  }

  async readyToggle() {
    await this.tournaments.readyMatch();
    this.readyColor = 'btn-success';
  }

  async startMatch(seconds: number) {
    await this.tournaments.readyMatch();
    if (seconds > 0) {
      return this.tournaments.startTimedMatch(seconds * 1000);
    } else {
      return this.tournaments.startMatch();
    }
  }

  stopMatch() {
    return this.tournaments.stopMatch();
  }

}
