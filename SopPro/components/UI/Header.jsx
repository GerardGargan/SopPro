import { View, Text, StyleSheet } from "react-native";
import React from "react";

const Header = ({ text, textStyle, containerStyle }) => {
  return (
    <View style={[styles.rootContainer, containerStyle && containerStyle]}>
      <Text style={[styles.headerText, textStyle && textStyle]}>{text}</Text>
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
