import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';

import { MocService, Moc, Competitor, CompetitorService } from '@bab/core';

@Component({
  selector: 'app-moc',
  templateUrl: './moc.component.html',
  styleUrls: ['./moc.component.css']
})
export class MocComponent implements OnInit {
  private moc$: Observable<Moc>;
  private isNew: boolean;
  private id: number;
  private image: File;

  constructor(
    private mocs: MocService,
    private competitors: CompetitorService,
    private route: ActivatedRoute,
    private router: Router,
    ) { }

  ngOnInit() {
    this.moc$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        this.isNew = this.id <= 0;
      }),
      switchMap(_ => {
        if (this.isNew) {
          return new Observable<Moc>(
            observer => {
              observer.next(new Moc());
              observer.complete();
            }
          );
        }
        return this.mocs.get(this.id);
      })
    );
  }

  changeImage(event) {
    this.image = event.target.files[0];
  }

  async save(moc: Moc, competitor: Competitor) {
    moc.competitorId = await this.saveCompetitor(competitor);
    return await this.saveMoc(moc);
  }

  async saveMoc(moc: Moc) {
    if (this.isNew) {
      const result = await this.mocs.create(moc);
      await this.saveImage(result);
      this.router.navigate(['../' + result], { relativeTo: this.route });
    } else {
      await this.saveImage(moc._id);
      return this.mocs.update(moc);
    }
  }

  async saveImage(id: number) {
    console.log(this.image);
    if (this.image) {
      return this.mocs.uploadImage(id, this.image);
    }
  }

  async saveCompetitor(competitor: Competitor): Promise<number> {
    if (!competitor) {
      return 0;
    }
    if (competitor._id) {
      return competitor._id;
    }
    return this.competitors.create(competitor);
  }

  foundCompetitor(event, competitor: Competitor) {
    if (event.target) {
      return;
    } else if (typeof event === 'string') {
      competitor.name = event;
    } else {
      competitor = event;
    }
  }

  setClass(event, moc: Moc) {
    if (event.target) {
      return;
    }
    const id = Number(event);
    if (id) {
      moc.classificationId = id;
    }
  }

}
