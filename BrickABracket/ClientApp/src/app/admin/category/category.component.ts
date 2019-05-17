import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap, tap, shareReplay, map, filter, take } from 'rxjs/operators';

import { TournamentService, Category, TournamentMetadata } from '@bab/core';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {

  private category$: Observable<Category>;
  private mocIds$: Observable<Array<number>>;
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
        this.tournaments.setCategory(this.id);
      }),
      switchMap(_ => this.tournaments.category),
      shareReplay(1)
    );
    this.mocIds$ = this.category$.pipe(
      filter(c => !!c),
      map(c => c.mocIds),
      );
  }

  nextMatch() {
    this.tournaments.nextMatch().then(_ => {
      this.tournaments.metadata.pipe(take(1)).subscribe(md => {
        this.router.navigate(['../rounds/matches/' + md.matchIndex], { relativeTo: this.route });
      });
    });
  }

  // TODO: Standings display

}
