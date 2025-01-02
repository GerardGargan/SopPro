import { Text, View } from "react-native";
import React from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useRouter } from "expo-router";
import { Button } from "react-native-paper";

const index = () => {
  const isFocused = useIsFocused();
  const router = useRouter();
  return (
    <>
      <View style={{ justifyContent: "center", alignItems: "center" }}>
        <Text>Home</Text>
        <Text>Page content to be added...</Text>
      </View>
      {isFocused && <Fab />}
    </>
  );
};

export default index;
