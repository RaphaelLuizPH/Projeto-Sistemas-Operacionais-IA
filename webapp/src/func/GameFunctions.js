import axios from "axios";

const api_url = import.meta.env.VITE_APP_API_URL;

export async function CreateGame() {
  return axios.post(`${api_url}/api/Game/Create`);
}

export async function SendMessage(id, message, suspectName) {
  return axios.post(`${api_url}/api/Game/Send/${id}`, null, {
    params: {
      id: id,
      message: message,
      suspectName: suspectName,
    },
    headers: {
      "Content-Type": "application/json",
    },
  });
}

export async function StartGame(id) {
  return axios.patch(`${api_url}/api/Game/Start`, null, {
    params: { id },
  });
}

export async function ListGames() {
  return axios.get(`${api_url}/api/Game/List`);
}

export async function GetGame(id) {
  return axios.get(`${api_url}/api/Game/Get/${id}`);
}
