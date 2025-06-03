import * as signalR from "@microsoft/signalr";

let connection;

export function getConnection() {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5014/gamehub")
      .build();
  }

  return connection;
}
