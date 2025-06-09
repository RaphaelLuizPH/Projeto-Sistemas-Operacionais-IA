import { ListGames } from "./func/GameFunctions";
import "./App.css";
import { motion } from "motion/react";
import { Button, ConfigProvider, theme } from "antd";
import {
  Route,
  BrowserRouter as Router,
  Routes,
  useLocation,
  Link,
} from "react-router-dom";
import StartScreen from "./pages/StartScreen";
import { AnimatePresence, delay } from "motion/react";
import GamePage from "./pages/GamePage";
import EndScreen from "./pages/EndScreen";
const getGames = async () => {
  try {
    const response = await ListGames();
    console.log(response);
  } catch (error) {
    console.error("Error fetching games:", error);
  }
};

getGames();

function App() {
  const location = useLocation();

  return (
    <>
      <AnimatePresence mode="wait">
        <ConfigProvider theme={{ algorithm: theme.darkAlgorithm }}>
          <Routes key={location.pathname} location={location}>
            <Route
              path="/"
              element={
                <AnimateWrapper>
                  <StartScreen />
                </AnimateWrapper>
              }
            />
            <Route
              path="/game/:id"
              element={
                <AnimateWrapper>
                  <GamePage />
                </AnimateWrapper>
              }
            />
            <Route
              path="/game/end/:id/:suspect"
              element={
                <AnimateWrapper>
                  <EndScreen />
                </AnimateWrapper>
              }
            />
          </Routes>
        </ConfigProvider>
      </AnimatePresence>
    </>
  );
}

export default App;

const AnimateWrapper = ({ children }) => {
  return (
    <motion.div
      initial={{ y: -5000 }}
      animate={{ y: 0 }}
      exit={{ y: -1000, transition: { duration: 1.2 } }}
      transition={{ duration: 1, type: "spring", delay: 0.1 }}
    >
      {children}
    </motion.div>
  );
};
