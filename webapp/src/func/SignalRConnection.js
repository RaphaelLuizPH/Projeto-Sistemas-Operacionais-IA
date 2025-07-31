import * as signalR from "@microsoft/signalr";

let connection;

export function getConnection() {
  if (!connection) {
    // Determine if we're in production or development
    const isProd = true;

    // Set the appropriate base URL
    const baseUrl = `${
      isProd ? import.meta.env.VITE_APP_API_URL : "http://localhost:5014"
    }/gamehub`;

    connection = new signalR.HubConnectionBuilder()
      .withUrl(baseUrl)
      .withAutomaticReconnect()
      .build();
  }

  return connection;
}
