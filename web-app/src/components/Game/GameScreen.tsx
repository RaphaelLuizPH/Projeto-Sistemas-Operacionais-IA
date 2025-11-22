import getConnection from "../../signalR/signalR";
import axiosSingleton from "../../axios/axios";
import * as signalR from "@microsoft/signalr";
import type { ChatMessage, ChatMessageRequest } from "../../types/ChatMessage";
import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router";
import { useNavigate } from "react-router";
export default function GameScreen() {
  const [chat, setChat] = useState<ChatMessage[]>([]);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );
  const { gameId } = useParams();
  const redirect = useNavigate();

  async function sendMessage(request: ChatMessageRequest): Promise<void> {
    connection?.invoke("SendMessage", request);
  }

  const connectSignalR = useCallback(
    async function connectSignalR(gameId: string) {
      if (
        connection &&
        connection.state === signalR.HubConnectionState.Connected
      ) {
        console.log("Joining game:", gameId);
        await connection.invoke("JoinGame", gameId);

        await connection.invoke("OpenChat", gameId, "1");
      }
    },
    [connection]
  );

  useEffect(() => {
    if (!gameId) {
      redirect("/");
      return;
    }

    axiosSingleton
      .post("api/Auth/token", { username: "Player", password: "" })
      .then((response) => {
        sessionStorage.setItem("token", response.data.token);
        getConnection()
          .then((conn) => {
            setConnection(conn);
            console.log("SignalR Connected.");
          })
          .then(() => connectSignalR(gameId));
      });

    return () => {
      function cleanup() {
        getConnection().then((conn) => {
          conn.invoke("LeaveGame", gameId);
          conn.stop();
        });
      }

      cleanup();
    };
  }, [connectSignalR]);

  useEffect(() => {
    if (!connection) return;

    connection.on("ReceiveChatHistory", (messages: ChatMessage[]) => {
      console.log("Received chat history:", messages);
      setChat(messages);
    });

    connection.on("ReceiveMessage", (user: string) => {
      console.log(`
    Message from ${user}`);
    });

    connection.on("Send", (newMessage: ChatMessage) => {
      console.log(`
    Message from ${newMessage.message}`);
      console.log(newMessage);
      setChat((prevChat) => [...prevChat, newMessage]);
    });
    console.log("Connecting to SignalR with game ID:", gameId);

    return () => {
      if (!connection) return;
      connection.off("ReceiveMessage");
      connection.off("Send");
      connection.off("ReceiveChatHistory");
    };
  }, [connection]);

  return <div>Game Screen</div>;
}
