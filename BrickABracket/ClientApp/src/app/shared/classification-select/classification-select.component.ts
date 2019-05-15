import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { ClassificationService, Classification } from '@bab/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-classification-select',
  templateUrl: './classification-select.component.html',
  styleUrls: ['./classification-select.component.css']
})
export class ClassificationSelectComponent implements OnInit {
  @Input()
  set classificationId(value: number) {
    this._selected = value;
    this.change.emit(value);
  }

  @Input()
  set elementId(value: string) {}

  @Output()
  change = new EventEmitter<number>();

  private _selected: number;

  private classifications$: Observable<Array<Classification>>;

  constructor(private classifications: ClassificationService) { }

  ngOnInit() {
    this.classifications$ = this.classifications.classifications;
  }

  onChange(newValue) {
    this.classificationId = newValue;
  }
}
