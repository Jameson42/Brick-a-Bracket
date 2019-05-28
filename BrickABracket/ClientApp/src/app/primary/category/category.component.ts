import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TournamentService, Category } from '@bab/core';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {

  private category$: Observable<Category>;

  constructor(
    private tournaments: TournamentService
  ) { }

  ngOnInit() {
    this.category$ = this.tournaments.category;
  }

}
