import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { Chip } from "react-native-paper";

const SopStatusChip = ({ status }) => {
  function getStatus(identifier) {
    switch (identifier) {
      case 1:
        return "Draft";
      case 2:
        return "In review";
      case 3:
        return "Approved";
      case 4:
        return "Archived";
      default:
        return "Unknown";
    }
  }

  return (
    <View style={styles.chip}>
      <Text style={styles.text}>{getStatus(status)}</Text>
    </View>
  );
};

export default SopStatusChip;

const styles = StyleSheet.create({
  chip: {
    borderColor: "#cccccc",
    borderWidth: 1,
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 16,
    backgroundColor: "#f0f0f0",
    alignSelf: "flex-start",
  },
  text: {
    fontSize: 14,
    color: "black",
  },
});
