import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Observer } from 'rxjs';
import { switchMap, tap, map, shareReplay, filter } from 'rxjs/operators';

import { Tournament, TournamentService } from '@bab/core';

@Component({
  selector: 'app-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.css']
})
export class TournamentComponent implements OnInit {

  private tournament$: Observable<Tournament>;
  private mocIds$: Observable<Array<number>>;
  private isNew: boolean;
  private id: number;

  constructor(
    private tournaments: TournamentService,
    private route: ActivatedRoute,
    private router: Router
    ) { }

  ngOnInit() {
    this.tournament$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        this.isNew = this.id < 0;
      }),
      switchMap(_ => {
        if (this.id === 0) {
          return this.tournaments.tournament.pipe(
            tap(t => this.router.navigate(['../' + t._id], { replaceUrl: true, relativeTo: this.route }))
          );
        }
        if (this.isNew) {
          return new Observable<Tournament>(
            (observer: Observer<Tournament>) => {
            observer.next(new Tournament());
            observer.complete();
          });
        }
        if (this.id) {
          this.tournaments.setActive(this.id);
        }
        return this.tournaments.tournament;
      }), shareReplay(1)
    );
    this.mocIds$ = this.tournament$.pipe(
      filter(t => !!t),
      map(t => t.mocIds)
    );
  }

  async save(tournament: Tournament) {
    if (this.isNew) {
      const result = await this.tournaments.create(tournament.name, tournament.tournamentType);
      this.router.navigate(['../' + result, { relativeTo: this.route }]);
    } else {
      return this.tournaments.update(tournament);
    }
  }
}
