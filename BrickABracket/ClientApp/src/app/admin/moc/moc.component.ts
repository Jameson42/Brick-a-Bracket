import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';

import { MocService, Moc } from '@bab/core';

@Component({
  selector: 'app-moc',
  templateUrl: './moc.component.html',
  styleUrls: ['./moc.component.css']
})
export class MocComponent implements OnInit {
  private moc$: Observable<Moc>;
  private isNew: boolean;
  private id: number;

  constructor(
    private mocs: MocService,
    private route: ActivatedRoute,
    private router: Router,
    ) { }

  ngOnInit() {
    // TODO: Set competitor name from id
    // Classification name too?
    this.moc$ = this.route.paramMap.pipe(
      tap(params => {
        this.id = Number(params.get('id'));
        this.isNew = this.id <= 0;
      }),
      switchMap(_ => {
        if (this.isNew) {
          return new Observable<Moc>(
            (observer => {
              observer.next(new Moc());
              observer.complete();
            })
          );
        }
        return this.mocs.get(this.id);
      })
    );
  }

  async save(moc: Moc) {
    if (this.isNew) {
      const result = await this.mocs.create(moc);
      if (this.router.url.indexOf('tournament') > 0)
        await this.mocs.addToTournament(result);
      this.router.navigate(['../' + result], { relativeTo: this.route });
    } else {
      if (this.router.url.indexOf('tournament') > 0)
        await this.mocs.addToTournament(moc._id);
      return this.mocs.update(moc);
    }
  }

}
