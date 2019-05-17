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
  editable: boolean;

  private mocs$: Observable<Array<Moc>>;

  constructor(
    private mocs: MocService,
    private router: Router,
    private route: ActivatedRoute,
    ) { }

  ngOnInit() {
    this.mocs$ = combineLatest(
      this.mocs.mocs,
      this.mocIds
      ).pipe(
        map(([mocs, ids]) => mocs.filter(m => ids.indexOf(m._id) >= 0))
      );
  }

  addMoc() {
    this.router.navigate(['../mocs/new'], { relativeTo: this.route });
  }

  delete(id: number) {
    this.mocs.delete(id);
  }

}
