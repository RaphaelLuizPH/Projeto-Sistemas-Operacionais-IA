import React, { useState } from "react";
import {
  EditOutlined,
  EllipsisOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import { Avatar, Card, Flex, Switch } from "antd";

function Profile({ suspect }) {
  const [loading, setLoading] = useState(true);

  return (
    <Flex
      gap="middle"
      align="center"
      justify="center"
      direction="column"
      className="flex flex-col items-center w-full lg:px-80"
      wrap
    >
      <Card
        loading={!loading}
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
