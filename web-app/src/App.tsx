import "./App.css";
import GameScreen from "./components/Game/GameScreen.tsx";

import HomeScreen from "./components/Home/HomeScreen.tsx";
import { Route, Routes, BrowserRouter as Router } from "react-router";
function App() {
  return (
    <>
      <Router>
        <Routes>
          <Route path="/" element={<HomeScreen />} />
          <Route path="game/:gameId" element={<GameScreen />} />
        </Routes>
      </Router>
    </>
  );
}

export default App;
