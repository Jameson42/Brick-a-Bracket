import { Component, OnInit, Input } from '@angular/core';
import { MocService } from 'src/app/core/mocs/moc.service';
import { Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { Moc } from 'src/app/core/mocs/moc';
import { TournamentService } from 'src/app/core/tournaments/tournament.service';

@Component({
  selector: 'app-moc-list',
  templateUrl: './moc-list.component.html',
  styleUrls: ['./moc-list.component.css']
})
export class MocListComponent implements OnInit {

  @Input() tournamentId: number;
  private mocs$: Observable<Array<Moc>>;

  constructor(
    private mocs: MocService,
    private tournaments: TournamentService,
    ) { }

  ngOnInit() {
    if (this.tournamentId) {
      this.mocs$ = combineLatest(
        this.mocs.mocs, 
        this.tournaments.tournament
        ).pipe(
          map(([ma, t]) => ma.filter(m => t.mocIds.indexOf(m._id) >= 0))
        );
    } else {
      this.mocs$ = this.mocs.mocs;
    }
    // TODO: Get competitor name!
  }

  addMoc() {
    // TODO: Go to new moc page based on current router url
  }

}
