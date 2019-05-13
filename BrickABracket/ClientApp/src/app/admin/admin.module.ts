import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { TournamentListComponent } from './tournament-list/tournament-list.component';
import { TournamentComponent } from './tournament/tournament.component';
import { ClassificationComponent } from './classification/classification.component';
import { RoundComponent } from './round/round.component';
import { MatchComponent } from './match/match.component';
import { MocComponent } from './moc/moc.component';
import { MocListComponent } from './moc-list/moc-list.component';
import { CompetitorComponent } from './competitor/competitor.component';
import { CompetitorListComponent } from './competitor-list/competitor-list.component';
import { FormsModule } from '@angular/forms';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { HomeComponent } from './home/home.component';
import { DeviceListComponent } from './device-list/device-list.component';
import { DeviceComponent } from './device/device.component';
import { ClassificationListComponent } from './classification-list/classification-list.component';
import { LayoutComponent } from './layout/layout.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    AdminRoutingModule
  ],
  declarations: [
    TournamentListComponent,
    TournamentComponent,
    ClassificationComponent,
    RoundComponent,
    MatchComponent,
    MocComponent,
    MocListComponent,
    CompetitorComponent,
    CompetitorListComponent,
    BreadcrumbComponent,
    HomeComponent,
    DeviceListComponent,
    DeviceComponent,
    ClassificationListComponent,
    LayoutComponent,
  ]
})
export class AdminModule { }
