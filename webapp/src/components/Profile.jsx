import React, { useState } from "react";
import { TableOutlined } from "@ant-design/icons";
import { Avatar, Button, Card, Flex, Switch } from "antd";
import { EndGame } from "../func/GameFunctions";
import { useNavigate, useParams } from "react-router-dom";

function Profile({ suspect }) {
  const [loading, setLoading] = useState(true);
  const apiUrl = import.meta.env.VITE_API_URL;
  const navigate = useNavigate();
  const { id } = useParams();
  const handleEnd = async () => {
    navigate(`/game/end/${id}/${suspect.name}`);
  };

  const stressColor =
    suspect.stressLevel < 0.5
      ? "green"
      : suspect.stressLevel < 0.8
      ? "yellow"
      : "red";

  return (
    <Flex
      gap="middle"
      align="center"
      justify="center"
      direction="column"
      className="flex flex-col items-center w-full lg:px-80 "
      wrap
    >
      <Card
        extra={
          <h1 className={` text-2xl font-bold text-${stressColor}-300 `}>
            {(suspect.stressLevel * 10).toFixed(0)}
          </h1>
        }
        actions={[
          <Button
            key="setting"
            onClick={handleEnd}
            color="danger"
            variant="filled"
            disabled={suspect.stressLevel < 0.5}
          >
            Acusar
          </Button>,
        ]}
        style={{
          width: "99%",
          flex: "1 1 auto",
          background: "#00000080",
          backdropFilter: "blur(6px)",
        }}
      >
        <Card.Meta
          className=" min-w-full "
          avatar={
            <Avatar
              src={`/src/assets/${suspect.imageCode}.jpeg`}
              shape="square"
              size="large"
            />
          }
          title={<p>{suspect.name}</p>}
          description={
            <>
              <p className=" text-white">{suspect.description}</p>
            </>
          }
        />
      </Card>
    </Flex>
  );
}
export default Profile;
