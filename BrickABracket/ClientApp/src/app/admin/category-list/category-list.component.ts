import { Component, OnInit } from '@angular/core';
import { TournamentService } from 'src/app/core/tournaments/tournament.service';
import { Observable } from 'rxjs';
import { Category } from 'src/app/core/tournaments/category';
import { map } from 'rxjs/operators';
import { ClassificationService } from 'src/app/core/classifications/classification.service';

@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css']
})
export class CategoryListComponent implements OnInit {

  private categories$: Observable<Array<Category>>;

  constructor(
    private tournaments: TournamentService,
    private classifications: ClassificationService
    ) { }

  ngOnInit() {
    this.categories$ = this.tournaments.tournament.pipe(
      map(t => t.categories)
    );
  }

  generateCategories() {
    return this.tournaments.generateCategories();
  }

}
