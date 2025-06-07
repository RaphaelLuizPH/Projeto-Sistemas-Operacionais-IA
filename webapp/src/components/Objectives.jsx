import { CheckCircleOutlined } from "@ant-design/icons";

function Objectives({ obj }) {

  


  const objectives = Object.entries(obj).map(([key, value]) => {
    return { description: key, completed: value };
  });

  return (
    <div className="h-full w-ful ml-30 p-10 overflow-y-scroll no-scrollbar">
      <h2 className="text-6xl font-bold mb-4 font-exile text-white">
        Objetivos
      </h2>
      {objectives?.map((objective, index) => (
        <ObjectiveCard key={index} objective={objective} />
      ))}
    </div>
  );
}

const ObjectiveCard = ({ objective }) => {
  return (
    <div className="bg-white p-4 rounded-lg shadow-[0_3px_10px_rgb(0,0,0,0.2)] mb-4 flex items-center justify-between">
      <p>{objective.description}</p>
      {objective.completed ? (
        <CheckCircleOutlined style={{ color: "green" }} />
      ) : null}
    </div>
  );
};

export default Objectives;
