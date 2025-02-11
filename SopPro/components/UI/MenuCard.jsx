import { StyleSheet, Text, TouchableOpacity } from "react-native";
import React from "react";

const MenuCard = ({ Icon, title, onPress }) => {
  return (
    <TouchableOpacity style={styles.rootContainer} onPress={onPress}>
      <Icon size={28} color="#888" style={styles.icon} />
      <Text style={styles.optionText} numberOfLines={1}>
        {title}
      </Text>
    </TouchableOpacity>
  );
};

export default MenuCard;

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
    fontSize: 18,
    color: "#333",
  },
});
