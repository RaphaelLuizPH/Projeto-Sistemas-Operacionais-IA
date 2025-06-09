import React, { useEffect, useState } from "react";
import { Button } from "antd";
import { useNavigate, useParams } from "react-router-dom";
import { getConnection } from "../func/SignalRConnection";
import * as signalR from "@microsoft/signalr";
import { EndGame } from "../func/GameFunctions";

const EndScreen = () => {
  const { id, suspect } = useParams();
  const navigate = useNavigate();
  const [hasEnded, setHasEnded] = useState(false);
  const [stats, setStats] = useState(null);
  const handleRestart = () => {
    navigate("/");
  };

  const handleEndGame = async () => {
    try {
      if (stats) return;
      const response = await EndGame(id, suspect);
      setStats(response.data);
      setHasEnded(true);
    } catch (error) {
      console.error("Error ending game:", error);
    }
  };

  useEffect(() => {
    handleEndGame();
  }, [id, suspect]);

  return (
    <>
      <div className="end-screen p-20 flex flex-col items-center justify-center h-fit min-h-screen  bg-secondary">
        <h1 className="font-exile text-white  text-[5rem]">Fim de jogo</h1>

        <Button
          type="primary"
          variant="filled"
          color="default"
          onClick={handleRestart}
        >
          Voltar ao menu
        </Button>

        {stats && (
          <div className="bg-highlight w-full h-fit mt-25 p-6">
            <div className="text-white flex flex-row gap-10 bg-primary p-6 rounded-lg justify-center items-start">
              <div>
                <h1 className="font-exile text-8xl text-white">
                  {stats.caseFile.title}
                </h1>
                <p>Tempo de conclusão: {stats.time}</p>
                {stats.isCorrectSuspect ? (
                  <p className="text-green-300">Acusou o suspeito correto.</p>
                ) : (
                  <p className="text-highlight">Acusou o suspeito errado.</p>
                )}
                {stats.justiceServed ? (
                  <p className="text-green-300">A justiça foi feita.</p>
                ) : (
                  <p className="text-highlight">A justiça não foi feita.</p>
                )}
                <h1 className="my-4 text-3xl">Objetivos</h1>
                {Object?.keys(stats.objetivos)?.map((objective, index) => (
                  <p key={index} className="mt-2 text-green-500">
                    {objective}
                  </p>
                ))}
              </div>

              <img
                className="w-60 h-60 object-cover grayscale-100 ml-auto"
                src={`/src/assets/${stats?.caseFile.victim.imageCode}.jpeg`}
                alt=""
              />
            </div>

            <div className="text-white text-sm">
              {stats.message.split("\n").map((line, index) => (
                <p key={index} className="mt-3">
                  {line}
                </p>
              ))}
            </div>
          </div>
        )}
      </div>
    </>
  );
};

export default EndScreen;
