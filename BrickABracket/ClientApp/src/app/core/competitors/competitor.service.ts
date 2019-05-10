import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { Competitor } from './competitor';
import { SignalrService } from '../signalr.service';

@Injectable()
export class CompetitorService {
    private competitors$: Observable<Array<Competitor>>;

    constructor(private _signalR: SignalrService) { }

    get competitors(): Observable<Array<Competitor>> {
        if (!this.competitors$) {
            this.competitors$ = this._signalR.invokeAndListenFor('GetCompetitors', 'ReceiveCompetitors');
        }
        return this.competitors$;
    }

    get(id: number): Observable<Competitor> {
        return this.competitors.pipe(
            map(competitorArray => competitorArray.filter(c => c._id === id)[0]),
            shareReplay(1)
        );
    }

    create(competitor: Competitor) {
        return this._signalR.invoke('CreateCompetitor', competitor);
    }

    update(competitor: Competitor) {
        return this._signalR.invoke('UpdateCompetitor', competitor);
    }

    delete(id: number) {
        return this._signalR.invoke('DeleteCompetitor', id);
    }
}