import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, shareReplay, distinctUntilKeyChanged } from 'rxjs/operators';

import { Competitor } from './competitor';
import { SignalrService } from '../signalr.service';

@Injectable()
export class CompetitorService {
    private competitors$: Observable<Array<Competitor>>;

    constructor(
        private _signalR: SignalrService,
        private _httpClient: HttpClient
    ) { }

    get competitors(): Observable<Array<Competitor>> {
        if (!this.competitors$) {
            this.competitors$ = this._signalR.invokeAndListenFor('GetCompetitors', 'ReceiveCompetitors');
        }
        return this.competitors$;
    }

    get(id: number): Observable<Competitor> {
        return this.competitors.pipe(
            map(competitorArray => competitorArray.filter(c => c._id === id)[0]),
            distinctUntilKeyChanged('name'),
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

    uploadCompetitorCSV(file: Blob) {
        const formData: FormData = new FormData();
        formData.append('file', file);
        return this._httpClient.post('/api/import/competitors', formData).subscribe();
    }

    searchNames(term: string): Observable<Array<Competitor>> {
        return this.competitors.pipe(
            map(cs =>
                cs.filter(c =>
                    c.name.toLowerCase().indexOf(term.toLowerCase()) > -1
                    ).slice(0, 10)
                )
        );
    }
}
