import { StyleSheet } from "react-native";
import { Button } from "react-native-paper";

import React from "react";

const CustomButton = ({ mode = "contained", children, ...props }) => {
  let labelStyle = {
    fontSize: 18,
    fontWeight: "600",
    letterSpacing: 0.5,
    color: "white",
  };

  let style = {
    borderRadius: 12,
    marginBottom: 16,
    backgroundColor: "#3b82f6",
    elevation: 4,
    shadowColor: "#3b82f6",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
  };

  if (mode === "outlined") {
    labelStyle = {
      fontSize: 18,
      fontWeight: "600",
      letterSpacing: 0.5,
      color: "#3b82f6",
    };

    style = {
      borderRadius: 12,
      borderColor: "#3b82f6",
      borderWidth: 2,
    };
  }

  return (
    <Button
      {...props}
      contentStyle={styles.buttonContent}
      labelStyle={labelStyle}
      style={style}
    >
      {children}
    </Button>
  );
};

export default CustomButton;

const styles = StyleSheet.create({
  buttonContent: {
    height: 56,
  },
});
