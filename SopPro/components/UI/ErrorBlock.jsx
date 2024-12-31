import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { Ionicons } from "@expo/vector-icons";

const ErrorBlock = ({ children }) => {
  return (
    <View style={styles.container}>
      <Ionicons name="alert-circle-outline" size={24} color="#ad3137" />
      <Text style={styles.text}>{children}</Text>
    </View>
  );
};

export default ErrorBlock;

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    alignItems: "center",
    gap: 8,
    marginVertical: 24,
    paddingHorizontal: 8,
    paddingVertical: 8,
    backgroundColor: "#fef1f1",
    borderWidth: 1,
    borderColor: "#ad3137",
    borderRadius: 8,
  },
  text: {
    color: "#ad3137",
  },
});
