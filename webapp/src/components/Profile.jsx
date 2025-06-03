import React, { useState } from "react";
import {
  EditOutlined,
  EllipsisOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import { Avatar, Card, Flex, Switch } from "antd";

function Profile({ suspect }) {
  const [loading, setLoading] = useState(true);
  const actions = [
    <SettingOutlined key="setting" />,
    <EditOutlined key="edit" />,
    <EllipsisOutlined key="ellipsis" />,
  ];

  return (
    <Flex
      gap="middle"
      align="center"
      justify="center"
      direction="row"
      className="flex flex-col items-center w-full lg:px-80"
      wrap
    >
      <Card
        loading={!loading}
        actions={actions}
        style={{ minWidth: "100%", flex: "2 0 100px" }}
      >
        <Card.Meta
          className=" min-w-full"
          avatar={
            <Avatar
              src={`/src/assets/${suspect.imageCode}.jpeg`}
              shape="square"
              size="large"
            />
          }
          title={suspect.name}
          description={
            <>
              <p>{suspect.description}</p>
            </>
          }
        />
      </Card>
    </Flex>
  );
}
export default Profile;
