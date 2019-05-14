import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

// SignalR
import '@aspnet/signalr';

import { SignalrService } from './signalr.service';
import { ClassificationService } from './classifications/classification.service';
import { CompetitorService } from './competitors/competitor.service';
import { DeviceService } from './devices/device.service';
import { MocService } from './mocs/moc.service';
import { TournamentService } from './tournaments/tournament.service';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
  ],
  providers: [
    SignalrService,
    ClassificationService,
    CompetitorService,
    DeviceService,
    MocService,
    TournamentService,
  ],
})
export class CoreModule { }
