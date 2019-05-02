import { Injectable } from '@angular/core';
import { Observable, combineLatest } from 'rxjs';
import { shareReplay, map } from 'rxjs/operators';

// SignalR
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

// Manages SignalR connection.

// TODO: Reconnect if disconnected?
// Would probably need to re-set listens and watch connection.onClose

@Injectable()
export class SignalrService {

  private connection: HubConnection;
  private promise: Promise<HubConnection>;
  private url = '/tournamentHub';

  constructor() {
    this.connect();
  }

  async connect() {
    if (!this.connection && !this.promise) {
      this.connection = new HubConnectionBuilder().withUrl(this.url).build();
      this.promise = this.connection.start().then(() => this.connection);
    }
    return await this.promise;
  }

  // Invokes hub method on subscription, stores replay.
  invokeAndListenFor<T>(method: string, listener: string, ...parameters: any[]): Observable<T> {
    return combineLatest(this.listenFor<T>(listener), this.invoke(method, ...parameters)).pipe(
      // Only pass the listenFor results
      map((x) => x[0]),
      // Keep the last result
      shareReplay(1)
    );
  }

   listenFor<T>(listener: string): Observable<T> {
     // Create an observable that outputs results from this.connection.on()
     return Observable.create((observer: any) => {
       this.connection.on(listener, (data: any) => {
         observer.next(data);
       });
     });
  }

  invoke(method: string, ...parameters: any[]): Promise<any> {
    return this.connection.invoke(method, ...parameters);
  }
}
