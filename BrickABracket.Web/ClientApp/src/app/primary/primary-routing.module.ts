import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TournamentComponent, CategoryComponent,
  RoundComponent, MatchComponent } from '@bab/primary';

const routes: Routes = [
  { path: '', redirectTo: 'tournament', pathMatch: 'full'},
  { path: '', component: TournamentComponent },
  { path: 'tournament', component: TournamentComponent },
  { path: 'category', component: CategoryComponent },
  { path: 'round', component: RoundComponent },
  { path: 'match', component: MatchComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PrimaryRoutingModule { }
