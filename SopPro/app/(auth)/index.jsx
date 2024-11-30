import { View, Text } from "react-native";
import React from "react";
import { useSelector } from "react-redux";
import { SafeAreaView } from "react-native-safe-area-context";

const index = () => {
  const isLoggedIn = useSelector(state => state.auth.isLoggedIn)
  return (
    <SafeAreaView>
      <Text>Logged in, this is a protected route</Text>
    </SafeAreaView>
  );
};

export default index;
