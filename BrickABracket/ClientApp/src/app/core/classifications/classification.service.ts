import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import {Classification} from './classification';
import {SignalrService} from '../signalr.service';

@Injectable()
export class ClassificationService {
    private classifications$: Observable<Array<Classification>>;

    constructor(private _signalR: SignalrService) { }

    get classifications(): Observable<Array<Classification>> {
        if (!this.classifications$) {
            this.classifications$ = this._signalR.invokeAndListenFor<Array<Classification>>('GetClassifications', 'ReceiveClassifications');
        }
        return this.classifications$;
    }

    get(id: number): Observable<Classification> {
        return this.classifications.pipe(
            map(classificationArray => classificationArray.filter(c => c._id === id)[0]),
            shareReplay(1)
        );
    }

    create(name: string) {
        return this._signalR.invoke('CreateClassification', name);
    }

    update(classification: Classification) {
        return this._signalR.invoke('UpdateClassification', classification);
    }

    delete(id: number) {
        return this._signalR.invoke('DeleteClassification', id);
    }
}
