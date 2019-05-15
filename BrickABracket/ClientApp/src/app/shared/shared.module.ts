import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClassificationPipe, CompetitorPipe, MocPipe, ConnectionResolver } from '@bab/shared';

@NgModule({
  imports: [
    CommonModule,
  ],
  providers: [
    ConnectionResolver,
  ],
  declarations: [CompetitorPipe, MocPipe, ClassificationPipe],
  exports: [CompetitorPipe, MocPipe, ClassificationPipe]
})
export class SharedModule { }
