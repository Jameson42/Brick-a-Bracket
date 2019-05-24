import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PrimaryRoutingModule } from '@bab/primary/primary-routing.module';
import { TournamentComponent, CategoryComponent,
  RoundComponent, MatchComponent } from '@bab/primary';

@NgModule({
  imports: [
    CommonModule,
    PrimaryRoutingModule
  ],
  declarations: [
    TournamentComponent,
    CategoryComponent,
    RoundComponent,
    MatchComponent
  ]
})
export class PrimaryModule { }
