import React from "react";
import { Stack } from "expo-router";
import { useTheme } from "react-native-paper";

const _layout = () => {
  const theme = useTheme();

  return (
    <>
      <Stack>
        <Stack.Screen name="invite" options={{ headerShown: true }} />
      </Stack>
    </>
  );
};

export default _layout;
