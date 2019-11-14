import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { TournamentService, Category,
  Tournament, RedirectService } from '@bab/core';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit {

  private tournament$: Observable<Tournament>;
  private category$: Observable<Category>;

  constructor(
    private tournaments: TournamentService,
    private redirect: RedirectService,
  ) { }

  ngOnInit() {
    this.redirect.start('primary');
    this.tournament$ = this.tournaments.tournament;
    this.category$ = this.tournaments.category;
  }

}
