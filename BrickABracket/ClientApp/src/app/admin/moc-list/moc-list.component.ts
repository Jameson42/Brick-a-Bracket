import { Component, OnInit, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';

import { Moc, MocService } from '@bab/core';

@Component({
  selector: 'app-moc-list',
  templateUrl: './moc-list.component.html',
  styleUrls: ['./moc-list.component.css']
})
export class MocListComponent implements OnInit {

  @Input()
  mocIds: Observable<Array<number>>;

  @Input()
  editable: boolean = false;

  private mocs$: Observable<Array<Moc>>;
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
    this.mocs$ = combineLatest(
      this.mocs.mocs.pipe(map(mocs => mocs.concat(this.bye))),
      this.mocIds
      ).pipe(
        map(([mocs, ids]) => {
          let mocList = new Array<Moc>();
          ids.forEach(id => {
            mocList.push(mocs.find(m => m._id === id));
          });
          return mocList;
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
