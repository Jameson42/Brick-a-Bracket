import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import { SignalrService, ClassificationService,
  CompetitorService, DeviceService,
  MocService, TournamentService,
  RedirectService,
 } from '@bab/core';

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
    RedirectService,
  ],
})
export class CoreModule { }
