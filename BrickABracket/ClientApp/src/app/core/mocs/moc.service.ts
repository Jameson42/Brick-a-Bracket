import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import {Moc} from './moc';
import {SignalrService} from '../signalr.service';

@Injectable()
export class MocService {
    private mocs$: Observable<Array<Moc>>;

    constructor(private _signalR: SignalrService) { }

    get mocs(): Observable<Array<Moc>> {
        if (!this.mocs$) {
            this.mocs$ = this._signalR.invokeAndListenFor<Array<Moc>>('GetMocs', 'ReceiveMocs');
        }
        return this.mocs$;
    }

    get(id: number): Observable<Moc> {
        return this.mocs.pipe(
            map(mocArray => mocArray.filter(m => m._id === id)[0]),
            shareReplay(1)
        );
    }

    create(moc: Moc) {
        return this._signalR.invoke('CreateMoc', moc);
    }

    update(moc: Moc) {
        return this._signalR.invoke('UpdateMoc', moc);
    }

    delete(id: number) {
        return this._signalR.invoke('DeleteMoc', id);
    }
}
