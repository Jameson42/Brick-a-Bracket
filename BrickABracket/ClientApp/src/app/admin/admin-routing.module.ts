import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TournamentListComponent } from './tournament-list/tournament-list.component';
import { TournamentComponent } from './tournament/tournament.component';

const routes: Routes = [
  { path: '', component: TournamentListComponent },
  { path: 'tournament', component: TournamentComponent },
  { path: 'tournament/new', component: TournamentComponent },
  { path: 'tournament/:id', component: TournamentComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
