import { useEffect, useState } from "react";
import axiosSingleton from "../../axios/axios";
import type { GenericResult } from "../../types/GenericResult";
import { useNavigate } from "react-router";
export default function GameList() {
  async function fetchGames(): Promise<string[]> {
    const response = await axiosSingleton.get<GenericResult<string[]>>(
      "api/Games"
    );

    return response.data.result?.value ?? [];
  }

  const [games, setGames] = useState<string[]>([]);

  const redirect = useNavigate();

  const openGame = (gameId: string) => {
    redirect(`/game/${gameId}`);
  };

  useEffect(() => {
    fetchGames().then(setGames);
  }, []);

  return (
    <>
      <div className="h-full w-fit p-6 lg:w-[58%] overflow-y-auto">
        {games.length === 0 ? (
          <p className="text-cst-white mt-4 m-auto w-full text-center ">
            Nenhum jogo encontrado.
          </p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 text-cst-white">
            {games?.map((game) => (
              <GameListItem
                key={game}
                item={game}
                openGame={() => openGame(game)}
              />
            ))}
          </div>
        )}
      </div>
    </>
  );
}

interface GameListItemProps {
  item: string;
  openGame: () => void;
}

function GameListItem({ item, openGame }: GameListItemProps) {
  return (
    <div
      onClick={openGame}
      className="bg-cst-white/10 p-4 rounded-lg hover:bg-cst-white/20 cursor-pointer transition-all duration-200"
    >
      {item}
    </div>
  );
}
