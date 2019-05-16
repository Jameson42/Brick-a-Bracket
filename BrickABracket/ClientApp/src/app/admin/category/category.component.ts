import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { TournamentService, Category } from '@bab/core';
import { switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {

  private category$: Observable<Category>;
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
      switchMap(_ => this.tournaments.category)
    );
  }

  // TODO: Round generation
  // round display
  // MOC display
  // Standings display

}
