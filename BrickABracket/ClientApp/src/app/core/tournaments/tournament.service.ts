import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import {Tournament, TournamentMetadata, TournamentSummary} from './tournament';
import {SignalrService} from '../signalr.service';

@Injectable()
export class TournamentService {
    private metadata$: Observable<TournamentMetadata>;
    private summaries$: Observable<Array<TournamentSummary>>;

    constructor(private _signalR: SignalrService) { }

    get summaries(): Observable<Array<TournamentSummary>> {
        if (!this.summaries$) {
            // tslint:disable-next-line:max-line-length
            this.summaries$ = this._signalR.invokeAndListenFor<Array<TournamentSummary>>('GetTournamentSummaries', 'ReceiveTournamentSummaries');
        }
        return this.summaries$;
    }

    get metadata(): Observable<TournamentMetadata> {
        if (!this.metadata$) {
            this.metadata$ = this._signalR.invokeAndListenFor<TournamentMetadata>('GetTournament', 'ReceiveTournament');
        }
        return this.metadata$;
    }

    get tournament(): Observable<Tournament> {
        return this.metadata.pipe(
            map(tm => tm.Tournament),
            shareReplay(1)
        );
    }

    create(name: string, type: string) {
        return this._signalR.invoke('CreateTournament', name, type);
    }

    update(tournament: Tournament) {
        return this._signalR.invoke('UpdateTournament', tournament);
    }

    delete(id: number) {
        return this._signalR.invoke('DeleteTournament', id);
    }

    setActive(id: number) {
        return this._signalR.invoke('SetActiveTournament', id);
    }

    generateCategories() {
        return this._signalR.invoke('GenerateCategories');
    }

    setCategory(index: number) {
        return this._signalR.invoke('SetCategoryIndex', index);
    }

    setRound(index: number) {
        return this._signalR.invoke('SetRoundIndex', index);
    }

    setMatch(index: number) {
        return this._signalR.invoke('SetMatchIndex', index);
    }

    nextMatch() {
        return this._signalR.invoke('NextMatch');
    }

    readyMatch() {
        return this._signalR.invoke('ReadyMatch');
    }

    startMatch() {
        return this._signalR.invoke('StartMatch');
    }

    startTimedMatch(ms: number) {
        return this._signalR.invoke('StartTimedMatch', ms);
    }

    stopMatch() {
        return this._signalR.invoke('StopMatch');
    }
}
