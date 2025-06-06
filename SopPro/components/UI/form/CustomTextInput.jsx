import { StyleSheet } from "react-native";
import React from "react";
import { TextInput, useTheme } from "react-native-paper";

// Custom text input which inherits the themes styling
const CustomTextInput = ({ ...props }) => {
  const theme = useTheme();

  return (
    <TextInput
      mode="outlined"
      style={styles.input}
      outlineColor={theme.colors.outline}
      activeOutlineColor={theme.colors.primary}
      placeholderTextColor="#666"
      {...props}
    />
  );
};

export default CustomTextInput;

const styles = StyleSheet.create({
  input: {
    height: 60,
    fontSize: 16,
  },
});
