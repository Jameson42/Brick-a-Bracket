import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { DeviceOptions, DeviceMetadata, DeviceRole } from './device';
import { SignalrService } from '../signalr.service';

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

    getDeviceOptions(): Observable<Array<DeviceOptions>> {
        return this._signalR.invokeAndListenFor<Array<DeviceOptions>>('GetDeviceOptions', 'ReceiveDeviceOptions');
    }

    get(connection: string): Observable<DeviceMetadata> {
        return this.devices.pipe(
            map(deviceArray => deviceArray.filter(d => d.connectionString === connection)[0]),
            shareReplay(1)
        );
    }

    create(deviceData: DeviceMetadata) {
        return this._signalR.invoke('CreateDevice', deviceData.connectionString,
            deviceData.program, deviceData.deviceType, deviceData.role);
    }

    setRole(connection: string, role: DeviceRole) {
        return this._signalR.invoke('SetDeviceRole', connection, role);
    }
    setProgram(connection: string, program: string) {
        return this._signalR.invoke('SetDeviceProgram', connection, program);
    }

    delete(connection: string) {
        return this._signalR.invoke('DeleteDevice', connection);
    }
}
