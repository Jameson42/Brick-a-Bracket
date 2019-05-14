import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MocPipe } from './moc.pipe';
import { ClassificationPipe } from './classification.pipe';
import { CompetitorPipe } from './competitor.pipe';
import { CoreModule } from '../core/core.module';
import { ConnectionResolver } from './signalr-connection.resolver';

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
