import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AdminRoutingModule, BreadcrumbComponent,
  CategoryComponent, CategoryListComponent,
  ClassificationComponent, ClassificationListComponent,
  CompetitorComponent, CompetitorListComponent,
  DeviceComponent, DeviceListComponent,
  HomeComponent, LayoutComponent,
  MatchComponent, MocComponent,
  MocListComponent, RoundComponent,
  TournamentComponent, TournamentListComponent
} from '@bab/admin';

import { SharedModule } from '@bab/shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    AdminRoutingModule,
    SharedModule,
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
    CategoryListComponent,
    CategoryComponent,
  ]
})
export class AdminModule { }
