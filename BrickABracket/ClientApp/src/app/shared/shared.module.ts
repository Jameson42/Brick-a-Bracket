import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { ClassificationPipe, CompetitorPipe,
  MocPipe, ConnectionResolver,
  CompetitorTypeaheadComponent,
  ClassificationSelectComponent,
  MocImageComponent, LogPipe,
  BatteryPipe, LogoFullscreenComponent } from '@bab/shared';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgbModule,
  ],
  providers: [
    ConnectionResolver,
  ],
  declarations: [
    CompetitorPipe,
    MocPipe,
    ClassificationPipe,
    CompetitorTypeaheadComponent,
    ClassificationSelectComponent,
    MocImageComponent,
    LogPipe,
    BatteryPipe,
    LogoFullscreenComponent,
  ],
  exports: [
    CompetitorPipe,
    MocPipe,
    ClassificationPipe,
    LogPipe,
    BatteryPipe,
    CompetitorTypeaheadComponent,
    ClassificationSelectComponent,
    MocImageComponent,
    LogoFullscreenComponent,
  ]
})
export class SharedModule { }
