import { StyleSheet } from "react-native";
import { Button, useTheme } from "react-native-paper";

import React from "react";

const CustomButton = ({
  mode = "contained",
  height,
  children,
  style,
  ...props
}) => {
  const theme = useTheme();

  const buttonContent = {
    height: height || 40,
  };

  let labelStyle = {
    fontSize: 18,
    fontWeight: "600",
    letterSpacing: 0.5,
    color: "white",
  };

  let styles = {
    borderRadius: 12,
    marginBottom: 16,
    backgroundColor: theme.colors.primary,
    elevation: 4,
    shadowColor: theme.colors.primary,
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
  };

  if (mode === "outlined") {
    labelStyle = {
      fontSize: 18,
      fontWeight: "600",
      letterSpacing: 0.5,
      color: theme.colors.primary,
    };

    styles = {
      borderRadius: 12,
      borderColor: theme.colors.primary,
      borderWidth: 2,
    };
  }

  return (
    <Button
      {...props}
      contentStyle={buttonContent}
      labelStyle={labelStyle}
      style={[styles, style && style]}
    >
      {children}
    </Button>
  );
};

export default CustomButton;
