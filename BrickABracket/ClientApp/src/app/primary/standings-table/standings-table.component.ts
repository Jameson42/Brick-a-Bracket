import { Component, OnInit, Input } from '@angular/core';
import { Moc, Standing, Score } from '@bab/core';

@Component({
  selector: 'app-standings-table',
  templateUrl: './standings-table.component.html',
  styleUrls: ['./standings-table.component.css']
})
export class StandingsTableComponent implements OnInit {

  @Input()
  mocIds: Array<Moc>;
  @Input()
  standings: Array<Standing>;

  constructor() { }

  ngOnInit() {
  }

}
