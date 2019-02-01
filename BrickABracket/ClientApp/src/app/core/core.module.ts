import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

// SignalR
import '@aspnet/signalr';

import { ConnectionResolver } from './signalr-connection.resolver';
import { SignalrService } from './signalr.service';

@NgModule({
  imports: [
    CommonModule,
  ],
  providers: [
    ConnectionResolver,
    SignalrService,
  ]
})
export class CoreModule { }
