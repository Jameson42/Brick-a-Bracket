import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SecondaryRoutingModule } from '@bab/secondary/secondary-routing.module';
import { TournamentComponent, CategoryComponent,
  RoundComponent, MatchComponent } from '@bab/primary';

@NgModule({
  imports: [
    CommonModule,
    SecondaryRoutingModule
  ],
  declarations: [
    TournamentComponent,
    CategoryComponent,
    RoundComponent,
    MatchComponent
  ]
})
export class SecondaryModule { }
