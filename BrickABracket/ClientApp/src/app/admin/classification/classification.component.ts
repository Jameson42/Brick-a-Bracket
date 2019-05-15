import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { Classification, ClassificationService } from '@bab/core';
import { tap, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-classification',
  templateUrl: './classification.component.html',
  styleUrls: ['./classification.component.css']
})
export class ClassificationComponent implements OnInit {

  private classification$: Observable<Classification>;
  private isNew: boolean;
  private id: number;

  constructor(
    private classifications: ClassificationService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.classification$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        this.isNew = this.id <= 0;
      }),
      switchMap(_ => {
        if (this.isNew) {
          return new Observable<Classification>(
            observer => {
              observer.next(new Classification());
              observer.complete();
            }
          );
        }
        return this.classifications.get(this.id);
      })
    );
  }

  async save(classification: Classification) {
    if (this.isNew) {
      await this.classifications.create(classification.name);
    } else {
      await this.classifications.update(classification);
    }
    this.router.navigate(['../'], { relativeTo: this.route });
  }

}
