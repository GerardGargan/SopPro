import { Text } from "react-native";
import React from "react";
import { useSelector } from "react-redux";
import { SafeAreaView } from "react-native-safe-area-context";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";

const index = () => {
  const isFocused = useIsFocused();
  const isLoggedIn = useSelector((state) => state.auth.isLoggedIn);
  return (
    <SafeAreaView>
      <Text>Logged in, this is a protected route</Text>
      {isFocused && <Fab />}
    </SafeAreaView>
  );
};

export default index;
