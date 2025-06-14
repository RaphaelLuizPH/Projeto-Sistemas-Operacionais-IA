import { Input } from "antd";
import { TypeAnimation } from "react-type-animation";
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
  const [cursor, setCursor] = useState(true);
  const { name, conversationHistory } = suspect;
  const handleSendMessage = async (value) => {
    if (value.trim() === "") return;
    setThinking(true);
    try {
      conversationHistory.push({
        role: "user",
        parts: [
          {
            text: value,
          },
        ],
      });
      await SendMessage(id, value, name);
    } catch (error) {
      console.error("Error sending message:", error);
    } finally {
      setThinking(false);
    }
  };

  const cursorRef = useRef(null);

  const showCursorAnimation = (show) => {
    if (!cursorRef.current) {
      return;
    }

    const el = cursorRef.current;
    if (show) {
      el.classList.add("custom-type-animation-cursor");
    } else {
      el.classList.remove("custom-type-animation-cursor");
    }
  };

  useEffect(() => {
    if (chatContainerRef.current) {
      chatContainerRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [suspect.conversationHistory]); // Trigger scrolling when conversationHistory changes

  var lastIndex = conversationHistory?.length - 1;

  return (
    <div className="relative  ml-auto height-full w-[50%] rounded flex justify-end outline-white outline-2 outline-solid bg-secondary flex-col p-4 ">
      <div className="overflow-y-scroll no-scrollbar relative">
        <div className="sticky top-0 left-0   ">
          <Profile suspect={suspect} />
        </div>
        {conversationHistory?.map((message, index) => {
          if (index === lastIndex && message.role !== "user") {
            return (
              <AnimatePresence key={index} mode="wait">
                <motion.div
                  initial={{ opacity: 0, x: -100 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ duration: 0.5, ease: "easeInOut" }}
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
                    <TypeAnimation
                      className={"custom-type-animation-cursor"}
                      ref={cursorRef}
                      splitter={(str) => str.split(/(?= )/)}
                      sequence={[
                        () => showCursorAnimation(true),
                        message.parts[0].text,
                        () => {
                          if (chatContainerRef.current) {
                            chatContainerRef.current.scrollIntoView({
                              behavior: "smooth",
                            });
                          }
                          showCursorAnimation(false);
                        },
                      ]}
                      wrapper="p"
                      speed={50}
                      cursor={false}
                    />
                  </div>
                </motion.div>
              </AnimatePresence>
            );
          }

          return (
            <AnimatePresence key={index} mode="wait">
              <motion.div
                initial={{ opacity: 0, x: -100 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.5, ease: "easeInOut" }}
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
        {thinking && <Typing />}
      </div>

      <div className="justify-self-end">
        <div
          ref={chatContainerRef}
          className="border-t-1 border-gray-100 opacity-[0.2] mt-6"
        />
        <Search
          multiple
          className=" ml-auto"
          variant="borderless"
          placeholder="Escreva sua mensagem..."
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

const Typing = () => (
  <div className="typing">
    <div className="typing__dot"></div>
    <div className="typing__dot"></div>
    <div className="typing__dot"></div>
  </div>
);
