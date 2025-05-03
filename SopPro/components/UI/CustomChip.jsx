import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { useTheme } from "react-native-paper";

// Reusable chip component
const CustomChip = ({ children, style }) => {
  const theme = useTheme();
  return <View style={[styles.chip, style && style]}>{children}</View>;
};

export default CustomChip;

const styles = StyleSheet.create({
  chip: {
    backgroundColor: "#F4F4F6",
    borderWidth: 0,
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 16,
    alignSelf: "flex-start",
  },
});
