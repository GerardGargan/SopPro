import { View, Text } from "react-native";
import React from "react";
import { useSelector } from "react-redux";

const index = () => {
  const isLoggedIn = useSelector(state => state.auth.isLoggedIn)
  return (
    <View>
      <Text>index {isLoggedIn && "Logged in"}</Text>
    </View>
  );
};

export default index;
