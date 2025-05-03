import React from "react";
import { Picker } from "@react-native-picker/picker";
import { useTheme } from "react-native-paper/src/core/theming";
import { View } from "react-native";

// Select picker which inherits from the custom theme
const SelectPicker = ({ children, ...props }) => {
  const theme = useTheme();
  return (
    <View
      style={{
        overflow: "hidden",
        borderWidth: 1,
        borderRadius: 4,
        borderColor: theme.colors.outline,
        marginBottom: 10,
      }}
    >
      <Picker
        style={{
          backgroundColor: theme.colors.background,
        }}
        {...props}
      >
        {children}
      </Picker>
    </View>
  );
};

export default SelectPicker;
