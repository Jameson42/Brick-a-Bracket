import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';

import { Competitor, CompetitorService } from '@bab/core';

@Component({
  selector: 'app-competitor-list',
  templateUrl: './competitor-list.component.html',
  styleUrls: ['./competitor-list.component.css']
})
export class CompetitorListComponent implements OnInit {

  private competitors$: Observable<Array<Competitor>>;

  constructor(
    private competitors: CompetitorService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.competitors$ = this.competitors.competitors;
  }

  add() {
    this.router.navigate(['./new'], { relativeTo: this.route });
  }

  delete(id: number) {
    this.competitors.delete(id);
  }

}
