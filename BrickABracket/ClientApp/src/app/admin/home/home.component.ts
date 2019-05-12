import { Component, OnInit } from '@angular/core';
import { TournamentService } from 'src/app/core/tournaments/tournament.service';
import { CompetitorService } from 'src/app/core/competitors/competitor.service';
import { MocService } from 'src/app/core/mocs/moc.service';
import { ClassificationService } from 'src/app/core/classifications/classification.service';
import { DeviceService } from 'src/app/core/devices/device.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  private tournamentCount:number=0;
  private competitorCount:number=0;
  private mocCount:number=0;
  private classificationCount:number=0;
  private deviceCount:number=0;

  constructor(
    private tournaments: TournamentService,
    private competitors: CompetitorService,
    private mocs: MocService,
    private classifications: ClassificationService,
    private devices: DeviceService
  ) { }

  ngOnInit() {
    this.tournaments.summaries.subscribe(t=>{
      this.tournamentCount = t.length;
    });
    this.competitors.competitors.subscribe(c => {
      this.competitorCount = c.length;
    });
    this.mocs.mocs.subscribe(m => {
      this.mocCount = m.length;
    });
    this.classifications.classifications.subscribe(c => {
      this.classificationCount = c.length;
    })
    this.devices.devices.subscribe(d => {
      this.deviceCount = d.length;
    })
  }

}
