import { Injectable } from '@angular/core';
import { Observable, combineLatest, interval } from 'rxjs';
import { shareReplay, map } from 'rxjs/operators';

// SignalR
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

// Manages SignalR connection.
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
      this.connection = new HubConnectionBuilder()
        .withUrl(this.url)
        .withAutomaticReconnect()
        .build();
      this.connection.keepAliveIntervalInMilliseconds = 15000;
      this.connection.serverTimeoutInMilliseconds = 30000;
      this.promise = this.connection.start().then(() => this.connection);
    }
    return await this.promise;
  }

  // Invokes hub method on subscription, stores replay.
  invokeAndListenFor<T>(method: string, listener: string, ...parameters: any[]): Observable<T> {
    return combineLatest([this.listenFor<T>(listener), this.invoke(method, ...parameters)]).pipe(
      // Only pass the listenFor results
      map((x) => x[0]),
      // Keep the last result
      shareReplay(1)
    );
  }

   listenFor<T>(listener: string): Observable<T> {
     // Create an observable that outputs results from this.connection.on()
     return new Observable(((observer: any) => {
       this.connection.on(listener, (data: any) => {
         observer.next(data);
       });
     }));
  }

  invoke(method: string, ...parameters: any[]): Promise<any> {
    return this.connection.invoke(method, ...parameters);
  }
}
