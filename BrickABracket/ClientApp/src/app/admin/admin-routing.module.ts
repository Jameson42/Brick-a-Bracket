import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TournamentListComponent } from './tournament-list/tournament-list.component';
import { TournamentComponent } from './tournament/tournament.component';
import { HomeComponent } from './home/home.component';
import { CompetitorListComponent } from './competitor-list/competitor-list.component';
import { MocComponent } from './moc/moc.component';
import { ClassificationComponent } from './classification/classification.component';
import { ClassificationListComponent } from './classification-list/classification-list.component';
import { DeviceListComponent } from './device-list/device-list.component';
import { DeviceComponent } from './device/device.component';
import { CompetitorComponent } from './competitor/competitor.component';
import { LayoutComponent } from './layout/layout.component';
import { MatchComponent } from './match/match.component';
import { RoundComponent } from './round/round.component';
import { CategoryComponent } from './category/category.component';

const routes: Routes = [
  { path: '', component: LayoutComponent, children: [
    { path: '', component: HomeComponent },
    { path: 'tournaments/categories/round/match', redirectTo: 'tournaments/categories/rounds/0', pathMatch: 'full' },
    { path: 'tournaments/categories/round/match/:id', component: MatchComponent },
    { path: 'tournaments/categories/round', redirectTo: 'tournaments/categories/0', pathMatch: 'full' },
    { path: 'tournaments/categories/round/:id', component: RoundComponent },
    { path: 'tournaments/categories', redirectTo: 'tournaments/0', pathMatch: 'full' },
    { path: 'tournaments/categories/:id', component: CategoryComponent },
    { path: 'tournaments/mocs', redirectTo: 'tournaments/0', pathMatch: 'full' },
    { path: 'tournaments/mocs/new', component: MocComponent },
    { path: 'tournaments/mocs/:id', component: MocComponent },
    { path: 'tournaments/new', component: TournamentComponent },
    { path: 'tournaments/:id', component: TournamentComponent },
    { path: 'tournaments', component: TournamentListComponent },
    { path: 'competitors/new', component: CompetitorComponent },
    { path: 'competitors/:id', component: CompetitorComponent },
    { path: 'competitors', component: CompetitorListComponent },
    { path: 'classifications/new', component: ClassificationComponent },
    { path: 'classifications/:id', component: ClassificationComponent },
    { path: 'classifications', component: ClassificationListComponent },
    { path: 'devices/new', component: DeviceComponent },
    { path: 'devices/:id', component: DeviceComponent },
    { path: 'devices', component: DeviceListComponent },
  ] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
