import { Resolve } from '@angular/router';
import { Injectable } from '@angular/core';
import { SignalrService } from './signalr.service';

import { HubConnection } from '@aspnet/signalr';

@Injectable()
export class ConnectionResolver implements Resolve<HubConnection> {

    constructor(private signalrService: SignalrService)  { }

    resolve() {
        console.log('ConnectionResolver. Resolving...');
        return this.signalrService.connect();
    }
}
