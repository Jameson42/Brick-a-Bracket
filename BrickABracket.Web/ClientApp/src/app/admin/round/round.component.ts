import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap, shareReplay, tap, map, filter, take } from 'rxjs/operators';

import { TournamentService, Category, Round, Standing } from '@bab/core';

@Component({
  selector: 'app-round',
  templateUrl: './round.component.html',
  styleUrls: ['./round.component.css']
})
export class RoundComponent implements OnInit {

  private category$: Observable<Category>;
  private round$: Observable<Round>;
  private mocIds$: Observable<Array<number>>;
  private standings$: Observable<Array<Standing>>;
  private id$: Observable<number>;
  private id: number;

  constructor(
    private tournaments: TournamentService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit() {
    this.category$ = this.tournaments.category;
    this.round$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        if (this.id >= 0) {
          this.tournaments.setRound(this.id);
        } // TODO: route to correct index
      }),
      switchMap(_ => this.tournaments.round),
      shareReplay(1)
    );
    this.mocIds$ = this.round$.pipe(
      filter(r => !!r),
      map(r => r.mocIds),
    );
    this.standings$ = this.round$.pipe(
      filter(r => !!r),
      map(r => r.standings),
    );
    this.id$ = this.tournaments.metadata.pipe(
      map(m => m.roundIndex)
    );
  }

  nextMatch() {
    this.tournaments.nextMatch().then(_ => {
      this.tournaments.metadata.pipe(take(1)).subscribe(md => {
        if (md.matchIndex > -1) {
          this.router.navigate(['../matches/' + md.matchIndex], { relativeTo: this.route });
        } else {
          this.router.navigate(['../matches'], { relativeTo: this.route });
        }
      });
    });
  }

}
