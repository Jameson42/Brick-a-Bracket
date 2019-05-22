import { Component, OnInit, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';

import { Moc, MocService, MatchResult, Standing, MocDisplay } from '@bab/core';

@Component({
  selector: 'app-moc-list',
  templateUrl: './moc-list.component.html',
  styleUrls: ['./moc-list.component.css']
})
export class MocListComponent implements OnInit {

  @Input()
  mocIds: Observable<Array<number>>;
  @Input()
  results: Observable<Array<MatchResult>>;
  @Input()
  standings: Observable<Array<Standing>>;
  @Input()
  editable = false;

  private mocs$: Observable<Array<MocDisplay>>;
  private bye: Moc;

  constructor(
    private mocs: MocService,
    private router: Router,
    private route: ActivatedRoute,
    ) { }

  ngOnInit() {
    this.bye = new Moc;
    this.bye._id = -1;
    this.bye.name = 'Bye';
    if (this.standings) {
      this.mocs$ = this.getMocStandings();
    } else if (this.results) {
      this.mocs$ = this.getMocResults();
    } else {
      this.mocs$ = this.getMocs();
    }
  }

  getMocs(): Observable<Array<MocDisplay>> {
    return combineLatest(
      this.mocs.mocs.pipe(map(mocs => mocs.concat(this.bye))),
      this.mocIds
      ).pipe(
        map(([mocs, ids]) => {
          const mocList = new Array<MocDisplay>();
          ids.forEach(id => {
            mocList.push(mocs.find(m => m._id === id) as MocDisplay);
          });
          return mocList;
        })
      );
  }

  getMocResults(): Observable<Array<MocDisplay>> {
    return combineLatest(
      this.mocs.mocs.pipe(map(mocs => mocs.concat(this.bye))),
      this.mocIds,
      this.results,
      ).pipe(
        map(([mocs, ids, results]) => {
          const mocList = new Array<MocDisplay>();
          for (let i = 0; i < ids.length; i++ ) {
            const temp = mocs.find(m => m._id === ids[i]) as MocDisplay;
            temp.scores = results.map(r => r.scores.find(s => s.player === i));
            mocList.push(temp);
          }
          return mocList;
        })
      );
  }

  getMocStandings(): Observable<Array<MocDisplay>> {
    return combineLatest(
      this.mocs.mocs.pipe(map(mocs => mocs.concat(this.bye))),
      this.mocIds,
      this.standings,
      ).pipe(
        map(([mocs, ids, standings]) => {
          const mocList = new Array<MocDisplay>();
          for (let i = 0; i < ids.length; i++ ) {
            const temp = mocs.find(m => m._id === ids[i]) as MocDisplay;
            temp.standing = standings.find(s => s.mocId === ids[i]);
            mocList.push(temp);
          }
          return mocList.sort((a, b) => {
            if (!a.standing || !b.standing) {
              return 0;
            }
            return a.standing.place - b.standing.place;
          });
        })
      );
  }

  addMoc() {
    this.router.navigate(['../mocs/new'], { relativeTo: this.route });
  }

  delete(id: number) {
    this.mocs.delete(id);
  }

}
