import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import {Tournament, TournamentMetadata, TournamentSummary} from './tournament';
import {SignalrService} from '../signalr.service';
import { Category } from './category';
import { Round } from './round';
import { Match } from './match';

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
            map(tm => tm.tournament),
            shareReplay(1)
        );
    }

    get category(): Observable<Category> {
        return this.metadata.pipe(
            map(tm => {
                if (tm.categoryIndex >= 0) {
                    return tm.tournament.categories[tm.categoryIndex];
                }
                return null;
            }),
            shareReplay(1)
        );
    }

    get round(): Observable<Round> {
        return this.metadata.pipe(
            map(tm => {
                if (tm.categoryIndex >= 0 && tm.roundIndex >= 0) {
                    return tm.tournament.categories[tm.categoryIndex].rounds[tm.roundIndex];
                }
                return null;
            }),
            shareReplay(1)
        );
    }

    get match(): Observable<Match> {
        return this.metadata.pipe(
            map(tm => {
                if (tm.categoryIndex >= 0 && tm.roundIndex >= 0 && tm.matchIndex >= 0 ) {
                    return tm.tournament.categories[tm.categoryIndex].rounds[tm.roundIndex].matches[tm.matchIndex];
                }
                return null;
            }),
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

    deleteCurrentMatch() {
        return this._signalR.invoke('DeleteCurrentMatch');
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

    runoff(count: number) {
        return this._signalR.invoke('Runoff', count);
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
