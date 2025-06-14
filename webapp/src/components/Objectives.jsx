import { CheckCircleOutlined } from "@ant-design/icons";
import { AnimatePresence } from "motion/react";
import { useState } from "react";
import { motion } from "motion/react";
function Objectives({ obj }) {
  const [show, setShow] = useState(false);

  const objectives = Object.entries(obj).map(([key, value]) => {
    return { description: key, completed: value };
  });

  return (
    <div className="h-full w-ful ml-30 p-10 overflow-y-scroll no-scrollbar">
      <h2
        onClick={() => setShow(!show)}
        className={`transition-all text-6xl font-bold mb-4 font-exile cursor-pointer text-white ${
          show ? "opacity-100" : "opacity-10"
        } hover:opacity-100`}
      >
        Objetivos
      </h2>
      <AnimatePresence>
        {show &&
          objectives?.map((objective, index) => (
            <ObjectiveCard key={index} objective={objective} index={index} />
          ))}
      </AnimatePresence>
    </div>
  );
}

const ObjectiveCard = ({ objective, index }) => {
  return (
    <motion.div
      initial={{ opacity: 0, y: -20 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -20 }}
      transition={{ duration: 0.2, delay: index / 10 }}
      className=" relative bg-white p-4 rounded-lg shadow-[0_3px_10px_rgb(0,0,0,0.2)] mb-4 flex items-center justify-between "
    >
      <p className="mr-10 text-wrap">{objective.description}</p>
      {objective.completed ? (
        <div className="absolute flex items-center justify-center h-full w-10 bg-green-500 right-0 rounded-br-lg rounded-tr-lg">
          <CheckCircleOutlined
            className="text-2xl"
            style={{ color: "white" }}
          />
        </div>
      ) : null}
    </motion.div>
  );
};

export default Objectives;
