import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

class BreadcrumbLink {
  public route: string;
  public name: string;
}

@Component({
  selector: 'breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {
  private links: Array<BreadcrumbLink>;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.url.subscribe(e => {
      const length = e.length;
      this.links = new Array<BreadcrumbLink>();
      for (var i=0;i<length;i++) {
        this.links.push({
          route: e.slice(0,i+1).map(u => u.path).join('/'),
          name: e[i].path
        });
      }
    });
  }

}
