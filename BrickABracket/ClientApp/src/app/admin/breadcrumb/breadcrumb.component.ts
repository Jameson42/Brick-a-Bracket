import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

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

  constructor(
    private router: Router
    ) { }

  ngOnInit() {
    this.processUrl(this.router.url);
    this.router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        this.processUrl(e.urlAfterRedirects);
      }
    });
  }

  processUrl(url:string) {
    const urlParts = url.substr(1).split('/');
    const length = urlParts.length;
    this.links = new Array<BreadcrumbLink>();
    for (var i=0;i<length-1;i++) {
      this.links.push({
        route: urlParts.slice(0,i+1).join('/'),
        name: urlParts[i]
      });
    }
  }
}
