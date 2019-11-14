import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, shareReplay, distinctUntilChanged, groupBy, switchMap } from 'rxjs/operators';

import {Moc, MocClassificationGrouping} from './moc';
import {SignalrService} from '../signalr.service';

@Injectable()
export class MocService {
    private mocs$: Observable<Array<Moc>>;

    constructor(private _signalR: SignalrService, private _httpClient: HttpClient) { }

    get mocs(): Observable<Array<Moc>> {
        if (!this.mocs$) {
            this.mocs$ = this._signalR.invokeAndListenFor<Array<Moc>>('GetMocs', 'ReceiveMocs');
        }
        return this.mocs$;
    }

    get(id: number): Observable<Moc> {
        return this.mocs.pipe(
            map(mocArray => mocArray.filter(m => m._id === id)[0]),
            distinctUntilChanged((x: Moc, y: Moc) => {
                return x._id === y._id && x.classificationId === y.classificationId &&
                x.competitorId === y.competitorId && x.name === y.name && x.weight === y.weight;
            }),
            shareReplay(1),
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

    getImageUrl(id: string): string {
        return '/api/mocs/' + id;
    }

    getImageBlob(id: number): Observable<Blob> {
        return this._httpClient.get(this.getImageUrl(id.toString()), { responseType: 'blob' });
    }

    uploadImage(id: number, image: Blob) {
        const formData: FormData = new FormData();
        formData.append('file', image);
        return this._httpClient.post('/api/mocs/' + id, formData).subscribe();
    }

    getClassificationGroupings(mocIds$: Observable<Array<number>>): Observable<Array<MocClassificationGrouping>> {
        return mocIds$.pipe(
            switchMap(ids => this.mocs.pipe(
                map(mocs => ids.map(id => mocs.find(moc => moc._id === id)))
            )),
            map(mocs =>
                mocs.reduce<Array<MocClassificationGrouping>>((accumulation, moc) => {
                    const index = accumulation.findIndex(mcg => mcg.classificationId === moc.classificationId);
                    if (index > -1) {
                        accumulation[index].mocs.push(moc);
                    } else {
                        accumulation.push(new MocClassificationGrouping(moc));
                    }
                    return accumulation;
                }, new Array<MocClassificationGrouping>()).sort((a, b) => a.classificationId - b.classificationId)
            )
        );
    }

}
