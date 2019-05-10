import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { TournamentListComponent } from './tournament-list/tournament-list.component';
import { TournamentComponent } from './tournament/tournament.component';
import { ClassificationComponent } from './classification/classification.component';
import { RoundComponent } from './round/round.component';
import { MatchComponent } from './match/match.component';
import { MocComponent } from './moc/moc.component';
import { MocListComponent } from './moc-list/moc-list.component';
import { CompetitorComponent } from './competitor/competitor.component';
import { CompetitorListComponent } from './competitor-list/competitor-list.component';

@NgModule({
  imports: [
    CommonModule,
    AdminRoutingModule
  ],
  declarations: [
    TournamentListComponent,
    TournamentComponent,
    ClassificationComponent,
    RoundComponent,
    MatchComponent,
    MocComponent,
    MocListComponent,
    CompetitorComponent,
    CompetitorListComponent,
  ]
})
export class AdminModule { }
