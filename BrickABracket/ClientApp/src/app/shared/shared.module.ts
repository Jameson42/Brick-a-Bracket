import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { ClassificationPipe, CompetitorPipe,
  MocPipe, ConnectionResolver,
  CompetitorTypeaheadComponent } from '@bab/shared';

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
  ],
  exports: [
    CompetitorPipe,
    MocPipe,
    ClassificationPipe,
    CompetitorTypeaheadComponent,
  ]
})
export class SharedModule { }
