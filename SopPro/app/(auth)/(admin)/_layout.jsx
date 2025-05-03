import React from "react";
import { Redirect, Stack } from "expo-router";
import { useSelector } from "react-redux";

const _layout = () => {
  const role = useSelector((state) => state.auth.role);

  // If the user is not an admin, dont render any admin screens - redirect to the home screen
  if (role !== "admin") {
    return <Redirect href="(auth)" />;
  }

  return (
    <>
      <Stack>
        <Stack.Screen name="invite" options={{ title: "Invite user" }} />
        <Stack.Screen
          name="department/departments"
          options={{ title: "Manage departments" }}
        />
        <Stack.Screen name="user/users" options={{ title: "Manage users" }} />
        <Stack.Screen name="user/[id]" options={{ title: "Edit user " }} />
        <Stack.Screen name="logo/index" options={{ title: "Custom logo" }} />
      </Stack>
    </>
  );
};

export default _layout;
