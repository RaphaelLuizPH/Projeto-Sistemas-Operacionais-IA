import { Flex, Progress } from "antd";
import { useEffect, useState } from "react";

function Loading({ time = 100 }) {
  const [percent, setPercent] = useState(0);

  const increasePercent = () => {
    if (percent === 90) {
      return;
    }

    setPercent((prev) => {
      if (prev < 100) {
        return prev + 10;
      }
      return prev;
    });
  };

  useEffect(() => {
    const interval = setInterval(increasePercent, time);
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="fixed top-0 left-0  h-[100vh] w-[100vw] flex items-center justify-center z-3 backdrop-blur-2xl">
      <Progress type="circle" percent={percent} />
    </div>
  );
}

export default Loading;
