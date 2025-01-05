import React from "react";
import { Picker } from "@react-native-picker/picker";
import { useTheme } from "react-native-paper/src/core/theming";
import { View } from "react-native";

const SelectPicker = ({ children, ...props }) => {
  const theme = useTheme();
  return (
    <View
      style={{
        borderBottomWidth: 1,
        borderBottomColor: theme.colors.outline,
        marginBottom: 10,
      }}
    >
      <Picker
        style={{
          backgroundColor: theme.colors.surfaceVariant,
        }}
        {...props}
      >
        {children}
      </Picker>
    </View>
  );
};

export default SelectPicker;
