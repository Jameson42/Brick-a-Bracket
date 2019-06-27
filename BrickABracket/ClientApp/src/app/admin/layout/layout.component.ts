import { Component, OnInit } from '@angular/core';
import { fadeAnimation } from 'src/app/fade.animation';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'],
  animations: [fadeAnimation]
})
export class LayoutComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  public getRouterOutletState(outlet) {
    return outlet.isActivated ? outlet.activatedRoute : '';
  }
}
