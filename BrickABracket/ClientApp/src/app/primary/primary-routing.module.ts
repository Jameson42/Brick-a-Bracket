import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: 'tournament', pathMatch: 'full'},
  { path: 'tournament' },
  { path: 'category' },
  { path: 'round' },
  { path: 'match' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PrimaryRoutingModule { }
