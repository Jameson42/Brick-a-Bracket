import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Competitor, CompetitorService } from '@bab/core';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-competitor-typeahead',
  templateUrl: './competitor-typeahead.component.html',
  styleUrls: ['./competitor-typeahead.component.css']
})
export class CompetitorTypeaheadComponent implements OnInit {
  get competitor(): Competitor {
    return this._competitor;
  }
  @Input() set competitor(value) {
    this._competitor = value;
    this.change.emit(value);
  }
  @Output() change = new EventEmitter<Competitor>();

  private _competitor: Competitor;
  private formatter = (x: Competitor) => x.name;
  private search = (text$: Observable<string>) => {
    return text$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => this.competitors.searchNames(term))
    );
  }

  constructor(private competitors: CompetitorService) { }

  ngOnInit() {
  }

}
