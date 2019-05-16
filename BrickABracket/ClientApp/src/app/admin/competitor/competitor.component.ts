import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';

import { Competitor, CompetitorService } from '@bab/core';

@Component({
  selector: 'app-competitor',
  templateUrl: './competitor.component.html',
  styleUrls: ['./competitor.component.css']
})
export class CompetitorComponent implements OnInit {

  private competitor$: Observable<Competitor>;
  private isNew: boolean;
  private id: number;

  constructor(
    private competitors: CompetitorService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.competitor$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        this.isNew = this.id <= 0;
      }),
      switchMap(_ => {
        if (this.isNew) {
          return new Observable<Competitor>(
            observer => {
              observer.next(new Competitor());
              observer.complete();
            }
          );
        }
        return this.competitors.get(this.id);
      })
    );
  }

  async save(competitor: Competitor) {
    if (this.isNew) {
      await this.competitors.create(competitor);
    } else {
      await this.competitors.update(competitor);
    }
    this.router.navigate(['../'], { relativeTo: this.route });
  }

}
