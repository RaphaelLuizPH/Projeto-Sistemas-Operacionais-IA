import { motion, stagger } from "motion/react";
import { useState } from "react";
import images from "../func/ImageImporter";
import { CreateGame, ListGames } from "../func/GameFunctions";
import { useNavigate } from "react-router-dom";
import { Drawer, List } from "antd";
import sleep from "../func/Sleep";
import "./StartScreen.css";
import Loading from "../components/Loading";

function StartScreen() {
  const colorChangeAnimation = {
    initial: { backgroundColor: "#var(--primary)" },
    animate: {
      backgroundColor: ["#070707", "var(--primary)", "#0a0101"],
      transition: {
        duration: 1,
        repeat: Infinity,
        repeatType: "reverse",
      },
    },
  };

  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const handleListGames = async () => {
    setLoading(true);
    try {
      const res = await ListGames();
      await sleep(1000);
      console.log("Games fetched successfully:", res.data);
      setGames(res.data);
      setOpen(true);
    } catch (error) {
      console.error("Error fetching games:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleStartGame = async () => {
    try {
      const response = await CreateGame();
      console.log("Game started successfully:", response);
      navigate(`/game/${response.data}`);
    } catch (error) {
      console.error("Error starting game:", error);
    }
  };

  const [open, setOpen] = useState(false);

  return (
    <>
      {loading && <Loading />}
      <motion.div
        className="start-menu-container w-full min-h-screen flex flex-col justify-around items-end"
        {...colorChangeAnimation}
      >
        <div className="text-white text-6xl lg:text-[10rem] md:text-[6rem] mx-auto block text-center mt-20 h-fit">
          <motion.h1
            className=" font-exile block mt-auto whitespace-nowrap"
            initial={{ opacity: 0, y: -100 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 2, ease: "easeInOut", type: "spring" }}
          >
            Investiga IA
          </motion.h1>

          <motion.div
            animate={[
              ["ul", { opacity: 1, y: 100 }, { duration: 0.5 }],
              ["li", { x: [-20, 0], opacity: [0, 1] }, { delay: stagger(0.2) }],
            ]}
            transition={{ duration: 0.5, ease: "easeInOut", type: "spring" }}
            className="text-white text-2xl lg:text-4xl md:text-3xl m-auto block text-center "
          >
            <ul className="menu mb-9 font-exile">
              <li onClick={handleStartGame}>Iniciar</li>
              <li onClick={() => handleListGames()}>Carregar</li>
              <li>Créditos</li>
              <li>Sair</li>
            </ul>
          </motion.div>
        </div>
        <div className="mt-auto overflow-hidden">
          <motion.div
            drag="x"
            dragConstraints={{ left: -600, right: 600 }}
            dragElastic={0.1}
            animate={{ x: [0, 100, 0] }}
            className="flex justify-center mt-auto  self-end p-8 w-screen"
          >
            {[...Object.keys(images)]
              .map((k) => ({ k, n: Math.random() }))
              .sort((a, b) => a.n - b.n)
              .map((item) => item.k)
              .map((key) => (
                <motion.img
                  layoutId={key}
                  draggable={false}
                  src={`${images[key]}`}
                  className="w-50 h-50 m-2 rounded-lg shadow-lg object-cover "
                  whileHover={{ scale: 1.1 }}
                />
              ))}
          </motion.div>
        </div>
      </motion.div>
      <Drawer
        title="Sessões ativas"
        placement={"bottom"}
        onClose={() => setOpen(false)}
        closable={true}
        open={open}
      >
        {Object.entries(games).map(([gameId, value]) => (
          <List.Item
            key={gameId}
            onClick={() => {
              navigate(`/game/${gameId}`);
            }}
            className="cursor-pointer"
          >
            {new Date(value.createdAt).toLocaleString("pt-BR", {
              year: "numeric",
              month: "2-digit",
              day: "2-digit",
              hour: "2-digit",
              minute: "2-digit",
            })}
          </List.Item>
        ))}
      </Drawer>
    </>
  );
}

export default StartScreen;
