import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { ClassificationService, CompetitorService, 
  DeviceService, TournamentService, RedirectService } from '@bab/core';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private tournamentCount$: Observable<number>;
  private competitorCount$: Observable<number>;
  private classificationCount$: Observable<number>;
  private deviceCount$: Observable<number>;

  constructor(
    private tournaments: TournamentService,
    private competitors: CompetitorService,
    private classifications: ClassificationService,
    private devices: DeviceService,
    private redirect: RedirectService,
  ) { }

  ngOnInit() {
    this.redirect.cancel();
    this.tournamentCount$ = this.tournaments.summaries.pipe(
      map(t => t.length)
    );
    this.competitorCount$ = this.competitors.competitors.pipe(
      map(c => c.length)
    );
    this.classificationCount$ = this.classifications.classifications.pipe(
      map(c => c.length)
    );
    this.deviceCount$ = this.devices.devices.pipe(
      map(d => d.length)
    );
  }

}
