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

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'tournament', component: TournamentListComponent },
  { path: 'tournament/new', component: TournamentComponent },
  { path: 'tournament/:id', component: TournamentComponent },
  { path: 'competitor', component: CompetitorListComponent },
  { path: 'competitor/new', component: CompetitorComponent },
  { path: 'competitor/:id', component: CompetitorComponent },
  { path: 'moc', component: MocListComponent },
  { path: 'moc/new', component: MocComponent },
  { path: 'moc/:id', component: MocComponent },
  { path: 'classification', component: ClassificationListComponent },
  { path: 'classification/new', component: ClassificationComponent },
  { path: 'classification/:id', component: ClassificationComponent },
  { path: 'device', component: DeviceListComponent },
  { path: 'device/new', component: DeviceComponent },
  { path: 'device/:id', component: DeviceComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
