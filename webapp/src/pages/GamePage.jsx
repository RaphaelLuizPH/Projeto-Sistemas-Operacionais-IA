import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { GetGame } from "../func/GameFunctions";
import { m, motion, stagger } from "motion/react";
import { SmallDashOutlined, LoadingOutlined } from "@ant-design/icons";
import images from "../func/ImageImporter";
import { StartGame } from "../func/GameFunctions";
import sleep from "../func/Sleep";
import "./GamePage.css";
function GamePage() {
  const { id } = useParams();
  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(false);
  useEffect(() => {
    fetchGame();
  }, [id]);

  const fetchGame = async () => {
    try {
      const response = await GetGame(id);
      setGame(response.data);
    } catch (error) {
      console.error("Error fetching game:", error);
    }
  };

  const handleStartGame = async () => {
    setLoading(true);
    try {
      StartGame(id);
      const response = await StartGame(id);
      console.log("Game started successfully:", response);
      sleep(5000);
      fetchGame();
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
    <div className="relative w-screen h-screen p-6 overflow-clip bg-accent flex">
      <motion.div
        initial={{ x: 1000 }}
        animate={{ x: -400, delay: 4 }}
        whileHover={{ x: 0, delay: 0.2 }}
        transition={{ duration: 0.5, ease: "easeInOut" }}
        className="border-solid border-30 border-highlight overflow-y-scroll max-h-full w-fit no-scrollbar "
      >
        <h1 className=" font-exile text-white text-[3rem] bg-highlight w-full">
          Suspeitos
        </h1>
        <motion.div className="flex flex-col overflow-y-scroll no-scrollbar">
          {game.suspects.map((s) => {
            console.log(s);
            return (
              <motion.img
                layoutId={`/src/assets/${s.imageCode}`}
                layout={"position"}
                key={s}
                src={`/src/assets/${s.imageCode}.jpeg`}
                className="object-cover h-60 w-100 shrink-0 grow-1"
              ></motion.img>
            );
          })}
        </motion.div>
      </motion.div>
    </div>
  );
}

export default GamePage;
