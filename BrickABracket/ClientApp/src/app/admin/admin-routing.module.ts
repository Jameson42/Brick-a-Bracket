import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TournamentListComponent } from './tournament-list/tournament-list.component';
import { TournamentComponent } from './tournament/tournament.component';
import { HomeComponent } from './home/home.component';
import { CompetitorListComponent } from './competitor-list/competitor-list.component';
import { MocComponent } from './moc/moc.component';
import { MocListComponent } from './moc-list/moc-list.component';
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
    { path: 'tournament/category/round/match/:id', component: MatchComponent },
    { path: 'tournament/category/round/:id', component: RoundComponent },
    { path: 'tournament/category/:id', component: CategoryComponent },
    { path: 'tournament/moc', redirectTo: 'tournament/0', pathMatch: 'full' },
    { path: 'tournament/moc/new', component: MocComponent },
    { path: 'tournament/moc/:id', component: MocComponent },
    { path: 'tournament/new', component: TournamentComponent },
    { path: 'tournament/:id', component: TournamentComponent },
    { path: 'tournament', component: TournamentListComponent },
    { path: 'competitor/new', component: CompetitorComponent },
    { path: 'competitor/:id', component: CompetitorComponent },
    { path: 'competitor', component: CompetitorListComponent },
    { path: 'classification/new', component: ClassificationComponent },
    { path: 'classification/:id', component: ClassificationComponent },
    { path: 'classification', component: ClassificationListComponent },
    { path: 'device/new', component: DeviceComponent },
    { path: 'device/:id', component: DeviceComponent },
    { path: 'device', component: DeviceListComponent },
  ] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
