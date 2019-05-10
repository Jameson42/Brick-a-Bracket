import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ConnectionResolver } from './core/signalr-connection.resolver';

const appRoutes: Routes = [
  { path: 'admin',
                loadChildren: './admin/admin.module#AdminModule',
                resolve: {connection: ConnectionResolver}},
  { path: 'primary',
                loadChildren: './primary/primary.module#PrimaryModule',
                resolve: {connection: ConnectionResolver}},
  { path: 'secondary',
                loadChildren: './secondary/secondary.module#SecondaryModule',
                resolve: {connection: ConnectionResolver}},
  { path: '', redirectTo: '/primary', pathMatch: 'full' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
