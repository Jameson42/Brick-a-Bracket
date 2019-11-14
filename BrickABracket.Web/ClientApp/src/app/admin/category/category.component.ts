import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap, tap, shareReplay, map, filter, take } from 'rxjs/operators';

import { TournamentService, Category, TournamentMetadata, Standing } from '@bab/core';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {

  private category$: Observable<Category>;
  private mocIds$: Observable<Array<number>>;
  private standings$: Observable<Array<Standing>>;
  private id: number;

  constructor(
    private tournaments: TournamentService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit() {
    this.category$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        if (this.id >= 0) {
          this.tournaments.setCategory(this.id);
        } // TODO: Route to correct index
      }),
      switchMap(_ => this.tournaments.category),
      shareReplay(1)
    );
    this.mocIds$ = this.category$.pipe(
      filter(c => !!c),
      map(c => c.mocIds),
    );
    this.standings$ = this.category$.pipe(
      filter(c => !!c),
      map(c => c.standings),
    );
  }

  nextMatch() {
    this.tournaments.nextMatch().then(_ => {
      this.tournaments.metadata.pipe(take(1)).subscribe(md => {
        if (md.matchIndex > -1) {
          this.router.navigate(['../rounds/matches/' + md.matchIndex], { relativeTo: this.route });
        } else {
          this.router.navigate(['../rounds/matches'], { relativeTo: this.route });
        }
      });
    });
  }

  runoff(count: number) {
    this.tournaments.runoff(count).then(_ => {
      this.tournaments.metadata.pipe(take(1)).subscribe(md => {
        if (md.roundIndex > -1) {
          this.router.navigate(['../rounds/' + md.roundIndex], { relativeTo: this.route });
        }
      });
    });
  }
}
