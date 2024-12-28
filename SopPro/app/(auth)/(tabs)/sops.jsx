import { StyleSheet, Text, View } from "react-native";
import React from "react";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";

const sops = () => {
  const isFocused = useIsFocused();

  return (
    <View>
      <Text>sops</Text>
      {isFocused && <Fab />}
    </View>
  );
};

export default sops;

const styles = StyleSheet.create({});
