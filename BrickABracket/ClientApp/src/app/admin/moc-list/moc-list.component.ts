import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-moc-list',
  templateUrl: './moc-list.component.html',
  styleUrls: ['./moc-list.component.css']
})
export class MocListComponent implements OnInit {

  @Input() tournamentId: number;

  constructor() { }

  ngOnInit() {
  }

}
