import { Component, OnInit } from '@angular/core';
import { SignalrService } from '../core/signalr.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  private service: SignalrService;
  constructor(private signalR: SignalrService) {
    this.service = signalR;
  }
  ngOnInit() {
    this.service
      .invokeAndListenFor('SendMessage', 'ReceiveMessage', 'Testing')
      .subscribe((message: string) => {
        alert(message);
      });
      // Never listen in the component directly like this unless you also do unsubscribe!
  }
}
