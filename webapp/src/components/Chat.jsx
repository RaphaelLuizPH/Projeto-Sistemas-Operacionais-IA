import { Input } from "antd";
import { SendOutlined } from "@ant-design/icons";
import { SendMessage } from "../func/GameFunctions";
import { useParams } from "react-router-dom";
import { useState, useEffect, useRef } from "react";
const { Search } = Input;
import { AnimatePresence, motion } from "motion/react";
import Profile from "./Profile";
function Chat({ suspect, setThinking, thinking }) {
  const chatContainerRef = useRef(null); // Ref for the chat container
  const { id } = useParams();
  const [message, setMessage] = useState([]);

  const { name, conversationHistory } = suspect;
  const handleSendMessage = async (value) => {
    if (value.trim() === "") return;
    setThinking(true);
    try {
      await SendMessage(id, value, name);
    } catch (error) {
      console.error("Error sending message:", error);
    } finally {
      setThinking(false);
    }
  };

  useEffect(() => {
    if (chatContainerRef.current) {
      chatContainerRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [suspect.conversationHistory]); // Trigger scrolling when conversationHistory changes

  return (
    <div className="relative  ml-auto height-full w-[50%] flex justify-end outline-white outline-2 outline-solid bg-secondary flex-col p-4 ">
      <div className="overflow-y-scroll no-scrollbar relative">
        <div className=" sticky top-0 left-0   ">
          <Profile suspect={suspect} />
        </div>
        {conversationHistory?.map((message, index) => {
          return (
            <AnimatePresence key={index} mode="wait">
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.3 }}
                key={index}
                className={`flex my-6  ${
                  message.role === "user" ? "justify-end" : "justify-start"
                }`}
              >
                <div
                  className={`p-2 rounded-lg max-w-[75%] ${
                    message.role === "user"
                      ? "bg-blue-500 text-white"
                      : "bg-gray-200 text-black"
                  }`}
                >
                  {message.parts[0].text}
                </div>
              </motion.div>
            </AnimatePresence>
          );
        })}

        {/* Empty div to scroll into view */}
      </div>

      <div className="justify-self-end">
        <div ref={chatContainerRef} style={{}} />
        <Search
          multiple
          className="mt-auto ml-auto"
          variant="borderless"
          placeholder="input search text"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          size="large"
          loading={thinking}
          enterButton={<SendOutlined style={{ color: "white" }} />}
          onSearch={(value) => {
            handleSendMessage(value);
            setMessage("");
          }}
        />
      </div>
    </div>
  );
}

export default Chat;
