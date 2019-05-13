import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import {Device, DeviceMetadata} from './device';
import {SignalrService} from '../signalr.service';

@Injectable()
export class DeviceService {
    private devices$: Observable<Array<DeviceMetadata>>;

    constructor(private _signalR: SignalrService) { }

    get devices(): Observable<Array<DeviceMetadata>> {
        if (!this.devices$) {
            this.devices$ = this._signalR.invokeAndListenFor<Array<DeviceMetadata>>('GetDevices', 'ReceiveDevices');
        }
        return this.devices$;
    }

    get(connection: string): Observable<DeviceMetadata> {
        return this.devices.pipe(
            map(deviceArray => deviceArray.filter(d => d.connectionString === connection)[0]),
            shareReplay(1)
        );
    }

    create(connection: string, program: string, type: string, role: string) {
        return this._signalR.invoke('CreateDevice', connection, program, type, role);
    }

    setRole(connection: string, role: string) {
        return this._signalR.invoke('SetDeviceRole', connection, role);
    }
    setProgram(connection: string, program: string) {
        return this._signalR.invoke('SetDeviceProgram', connection, program);
    }

    delete(connection: string) {
        return this._signalR.invoke('DeleteDevice', connection);
    }
}
