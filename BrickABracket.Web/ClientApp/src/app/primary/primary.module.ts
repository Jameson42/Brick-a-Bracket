import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PrimaryRoutingModule } from '@bab/primary/primary-routing.module';
import { TournamentComponent, CategoryComponent,
  RoundComponent, MatchComponent, StandingsTableComponent } from '@bab/primary';
import { SharedModule } from '@bab/shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    PrimaryRoutingModule,
    SharedModule,
  ],
  declarations: [
    TournamentComponent,
    CategoryComponent,
    RoundComponent,
    MatchComponent,
    StandingsTableComponent
  ]
})
export class PrimaryModule { }
