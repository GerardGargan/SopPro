import React from "react";
import { Redirect, Stack } from "expo-router";
import { useSelector } from "react-redux";

const _layout = () => {
  const role = useSelector((state) => state.auth.role);

  if (role !== "admin") {
    return <Redirect href="(auth)" />;
  }

  return (
    <>
      <Stack>
        <Stack.Screen name="invite" options={{ title: "Invite user" }} />
      </Stack>
    </>
  );
};

export default _layout;
