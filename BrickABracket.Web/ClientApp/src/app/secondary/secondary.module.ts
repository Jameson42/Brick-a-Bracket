import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SecondaryRoutingModule } from '@bab/secondary/secondary-routing.module';
import { TournamentComponent, CategoryComponent,
  RoundComponent, MatchComponent } from '@bab/secondary';
import { SharedModule } from '@bab/shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    SecondaryRoutingModule,
    SharedModule,
  ],
  declarations: [
    TournamentComponent,
    CategoryComponent,
    RoundComponent,
    MatchComponent
  ]
})
export class SecondaryModule { }
