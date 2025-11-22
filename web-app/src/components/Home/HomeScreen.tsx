import { useState } from "react";
import GameList from "./GameList";
import axiosSingleton from "../../axios/axios";
import type { GenericResult } from "../../types/GenericResult";
import { useNavigate } from "react-router";
export default function HomeScreen() {
  const [search, setSeach] = useState<boolean>(false);

  const redirect = useNavigate();

  const startGame = () => {
    axiosSingleton
      .post<GenericResult<string>>("api/Games/Create")
      .then((response) => {
        const gameId = response.data.result?.value;
        redirect(`/game/${gameId}`);
      });
  };

  const handleSearchClick = () => {
    setSeach((prev) => !prev);
  };

  return (
    <>
      <div className="flex w-screen h-screen items-start justify-between lg:flex-row flex-col">
        <div
          className="lg:w-[40%]  w-full h-full p-6"
          style={{ background: "var(--color-cst-hard)" }}
        >
          <h1 className="text-cst-white lg:text-6xl mb-12 font-fascinate m-auto text-center">
            INVESTIGA AI
          </h1>
          <ul className="list-none lg:text-[3rem] gap-0 *:cursor-pointer *:hover:text-cst-soft *:transition-all *:duration-200 font-jacquard text-cst-white">
            <li onClick={startGame}>Novo Jogo</li>
            <li onClick={handleSearchClick}>Procurar jogo</li>
          </ul>
        </div>

        {search && <GameList />}
      </div>
    </>
  );
}
