export type MessageType = "Model" | "User";

export const MessageType = {
  Model: "Model" as MessageType,
  User: "User" as MessageType,
};

export type ChatMessage = {
  message: string;
  messageID: string;
  sender: string;
  senderID: string;
  time: Date;
  type: MessageType;
};

export type ChatMessageRequest = {
  message: string;
  sender: string;
  gameId: string;
  chatId: string;
};
