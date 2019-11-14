import { Resolve } from '@angular/router';
import { Injectable } from '@angular/core';
import { HubConnection } from '@aspnet/signalr';

import { SignalrService } from '@bab/core';

@Injectable()
export class ConnectionResolver implements Resolve<HubConnection> {

    constructor(private signalrService: SignalrService)  { }

    resolve() {
        console.log('ConnectionResolver: Resolving...');
        return this.signalrService.connect();
    }
}
