import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import { FontAwesome6 } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import { Avatar } from "react-native-paper";

const SareAreaHeader = () => {
  const router = useRouter();
  return (
    <SafeAreaView style={styles.container}>
      <TouchableOpacity onPress={() => router.navigate("/more")}>
        <Avatar.Icon
          icon="account"
          color="black"
          size={35}
          style={styles.avatar}
        />
      </TouchableOpacity>
    </SafeAreaView>
  );
};

export default SareAreaHeader;

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    justifyContent: "flex-end",
    alignItems: "center",
    paddingHorizontal: 20,
    paddingVertical: 5,
  },
  avatar: {
    backgroundColor: "lightgrey",
    color: "white",
  },
});
