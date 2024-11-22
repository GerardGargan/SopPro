import { View, Text, StyleSheet } from "react-native";
import React from "react";

const Header = ({ text }) => {
  return (
    <View style={styles.rootContainer}>
      <Text style={styles.headerText}>{text}</Text>
    </View>
  );
};

const styles = StyleSheet.create({
  rootContainer: {
    marginVertical: 10,
    justifyContent: "center",
    alignItems: "center",
  },
  headerText: {
    fontSize: 30,
    fontWeight: "bold",
    color: "white",
    textAlign: "center",
  },
});

export default Header;
