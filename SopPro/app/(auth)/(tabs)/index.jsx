import { Text, View } from "react-native";
import React from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useRouter } from "expo-router";
import { Button } from "react-native-paper";
import Toast from "react-native-toast-message";

const index = () => {
  const isFocused = useIsFocused();
  const router = useRouter();
  const onPress = () => {
    Toast.show({
      type: "success",
      text1: "Button pressed",
      visibilityTime: 3000,
    });
  };
  return (
    <>
      <View style={{ justifyContent: "center", alignItems: "center" }}>
        <Text>Home</Text>
        <Text>Page content to be added...</Text>
        <Button onPress={onPress}>Press me</Button>
      </View>
      {isFocused && <Fab />}
    </>
  );
};

export default index;
