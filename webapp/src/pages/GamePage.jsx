import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { GetGame } from "../func/GameFunctions";
import { m, motion, stagger } from "motion/react";
import { SmallDashOutlined, LoadingOutlined } from "@ant-design/icons";
import images from "../func/ImageImporter";
import { StartGame } from "../func/GameFunctions";
import sleep from "../func/Sleep";
import Chat from "../components/Chat";
import * as signalR from "@microsoft/signalr";
import Objectives from "../components/Objectives";
import "./GamePage.css";
import { getConnection } from "../func/SignalRConnection";
import { Popover } from "antd";
import { FloatButton } from "antd";

function GamePage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [suspect, setSuspect] = useState(null);
  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(false);
  const [thinking, setThinking] = useState(false);
  const [timeElapsed, setTimeElapsed] = useState("00:00");
  const connection = getConnection();

  useEffect(() => {
    fetchGame();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  useEffect(() => {
    if (connection.state === signalR.HubConnectionState.Connected) {
      console.log("Already connected to SignalR");
      return;
    }

    connection.start().then(function () {
      console.log("Connected to SignalR");

      connection.invoke("JoinGame", id);
    });

    connection.on("ReceiveMessage", function (message) {
      setTimeElapsed(message);
    });

    connection.on("Response", function (res) {
      console.log("Response received:", res);
    });

    connection.on("Objectives", function (objectives) {
      setGame((prevGame) => ({
        ...prevGame,
        objectives: objectives,
      }));
    });

    connection.on("Conversation", function (conversationHistory) {
      setSuspect((prevSuspect) => {
        if (prevSuspect) {
          return { ...prevSuspect, conversationHistory: conversationHistory };
        }
        return suspect;
      });
    });

    return () => {
      const disconnect = async () => {
        try {
          await connection.invoke("LeaveGame", id);
          connection.off("ReceiveMessage");
          await connection.stop();
          console.log("Disconnected and left game:", id);
        } catch (err) {
          console.error("Error during disconnect:", err);
        }
      };

      disconnect();
    };
  }, [id]);

  const fetchGame = async () => {
    try {
      const response = await GetGame(id);
      setGame(response.data);
    } catch (error) {
      console.error("Error fetching game:", error);
      navigate("/");
    }
  };

  const handleStartGame = async () => {
    setLoading(true);
    try {
      const response = await StartGame(id);
      console.log("Game started successfully:", response);
      fetchGame();
      sleep(1000);
      setLoading(false);
    } catch (error) {
      console.error("Error starting game:", error);
      setLoading(false);
    }
  };

  if (!game?.suspects) {
    return (
      <div className="relative w-screen h-screen p-6 overflow-clip">
        <button className="start-button" onClick={handleStartGame}>
          {loading ? <LoadingOutlined /> : <SmallDashOutlined />}
          <p>Pronto para jogar?</p>
        </button>
        {Object.keys(images).map((key) => {
          const randomX = Math.random(0.5) * 100;
          const randomY = Math.random(0.5) * 100;
          console.log(key, images[key]);
          return (
            <motion.img
              initial={{ top: `${randomX}vh`, left: `${randomY}vw` }}
              layoutId={key}
              transition={{
                duration: 2,

                ease: "easeInOut",
              }}
              drag
              key={key}
              src={images[key]}
              className="object-cover h-50 w-50 shrink-0 absolute"
            ></motion.img>
          );
        })}
      </div>
    );
  }

  return (
    <div className="relative w-screen h-screen p-6 overflow-clip bg-accent flex justify-between">
      <motion.div
        initial={{ x: 1000 }}
        animate={{ x: -400, delay: 4 }}
        whileHover={{ x: 0, delay: 0.2 }}
        transition={{ duration: 0.5, ease: "easeInOut" }}
        className="border-solid border-30 border-highlight overflow-y-scroll max-h-full w-fit no-scrollbar absolute z-4"
      >
        <span className=" text-white text-[1rem] bg-highlight flex">
          {game.caseFile.title}
        </span>
        <h1 className=" font-exile text-white text-[3rem] bg-highlight w-full">
          Suspeitos{" "}
          <span className="font-serif text-2xl">{timeElapsed.time}</span>
        </h1>{" "}
        <motion.div className="flex flex-col overflow-y-scroll no-scrollbar">
          {game.suspects.map((s) => {
            if (s.name === game.caseFile.victim.name) {
              return (
                <Popover
                  content={` ${s.name} - A vítima`}
                  key={s.name}
                  placement="right"
                >
                  <motion.img
                    layoutId={`/src/assets/${s.imageCode}`}
                    layout={"position"}
                    key={s.name}
                    onClick={() => {
                      setSuspect(s);
                    }}
                    src={`/src/assets/${s.imageCode}.jpeg`}
                    className="object-cover h-100 w-100 shrink-0 grow-1 grayscale-100"
                  ></motion.img>
                </Popover>
              );
            }
            return (
              <Popover content={` ${s.name}`} key={s.name} placement="right">
                <motion.img
                  layoutId={`/src/assets/${s.imageCode}`}
                  layout={"position"}
                  key={s.name}
                  onClick={() => {
                    setSuspect(s);
                  }}
                  src={`/src/assets/${s.imageCode}.jpeg`}
                  className="object-cover h-100 w-100 shrink-0 grow-1"
                ></motion.img>
              </Popover>
            );
          })}
        </motion.div>
      </motion.div>
      <Objectives obj={game.objectives} />
      {suspect && (
        <Chat suspect={suspect} setThinking={setThinking} thinking={thinking} />
      )}
    </div>
  );
}

export default GamePage;
