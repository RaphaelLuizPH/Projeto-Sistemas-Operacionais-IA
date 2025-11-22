import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection;

async function getConnection(): Promise<signalR.HubConnection> {
  try {
    if (!connection) {
      connection = new signalR.HubConnectionBuilder()
        .withUrl(`${import.meta.env.VITE_APP_API_URL}gamehub`, {
          accessTokenFactory: () => sessionStorage.getItem("token") || "",
        })
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect()
        .build();
    }
  } catch (err) {
    console.log(err);
  }

  if (connection.state === signalR.HubConnectionState.Disconnected) {
    await connection.start();
  }

  return connection;
}

export default getConnection;
