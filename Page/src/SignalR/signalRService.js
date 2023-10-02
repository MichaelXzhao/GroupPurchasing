import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl("/GoShopHub")
  .withAutomaticReconnect()
  .build();

export default connection;