import { Component, OnInit } from '@angular/core';

import { ClassificationService, Classification } from '@bab/core';
import { Observable } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-classification-list',
  templateUrl: './classification-list.component.html',
  styleUrls: ['./classification-list.component.css']
})
export class ClassificationListComponent implements OnInit {

  private classifications$: Observable<Array<Classification>>;

  constructor(
    private classifications: ClassificationService,
    private router: Router,
    private route: ActivatedRoute
    ) { }

  ngOnInit() {
    this.classifications$ = this.classifications.classifications;
  }

  add() {
    this.router.navigate(['./new'], { relativeTo: this.route });
  }

}
