import { StyleSheet, Text, TouchableOpacity } from "react-native";
import React from "react";
import { FontAwesome5 } from "@expo/vector-icons";

const BottomSheetCard = ({ Icon, title, onPress }) => {
  return (
    <TouchableOpacity style={styles.rootContainer} onPress={onPress}>
      <Icon size={28} color="#888" style={styles.icon} />
      <Text style={styles.optionText} numberOfLines={1}>
        {title}
      </Text>
    </TouchableOpacity>
  );
};

export default BottomSheetCard;

const styles = StyleSheet.create({
  rootContainer: {
    flexDirection: "row",
    alignItems: "center",
    paddingVertical: 12,
    paddingHorizontal: 16,
    width: "100%",
  },
  icon: {
    width: 30,
    marginRight: 16,
  },
  optionText: {
    fontSize: 20,
    color: "#333",
  },
});
