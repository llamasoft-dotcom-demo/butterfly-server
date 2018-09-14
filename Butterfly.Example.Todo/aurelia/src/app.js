import { ArrayDataEventHandler, WebSocketChannelClient } from 'butterfly-client';

export class App {
  constructor() {
    this.message = 'Butterfly Server .NET Aurelia Todo Example'; 

    this.channelClient = null;
    this.channelClientState = null;
    this.todoList = [];
    this.channel = "todos";
    this.connect();
    this.subscribe();
  }

  connect() {
    let url = `ws://localhost:8000/ws`;
    this.channelClient = new WebSocketChannelClient({
      url,
      onStateChange(value) {
        this.channelClientState = value;
      }
    });

    this.channelClient.connect();
  }

  subscribe() {
    this.channelClient.subscribe({
      channel: this.channel,
      vars: {
        clientName: "AureliaWebClient"
      },
      handler: new ArrayDataEventHandler({
        arrayMapping: {
          todo: this.todoList
        }
      })
    });
  }

  unsubscribe() {
    this.channelClient.unsubscribe(this.channel);
  }
}
