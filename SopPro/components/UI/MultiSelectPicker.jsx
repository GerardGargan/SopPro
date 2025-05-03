import { StyleSheet } from "react-native";
import { MultiSelect } from "react-native-element-dropdown";
import { useTheme } from "react-native-paper";

import React from "react";

// Reusable component for selecting multiple items from a list
// Inherits from the theme
const MultiSelectPicker = ({ data, value, onChange }) => {
  const theme = useTheme();
  return (
    <MultiSelect
      style={[
        styles.dropdown,
        {
          backgroundColor: theme.colors.background,
          borderColor: theme.colors.outline,
        },
      ]}
      placeholderStyle={styles.placeholderStyle}
      selectedTextStyle={styles.selectedTextStyle}
      inputSearchStyle={styles.inputSearchStyle}
      search
      searchPlaceholder="Search..."
      data={data}
      labelField="name"
      valueField="id"
      placeholder="Select PPE"
      value={value || []}
      onChange={onChange}
      selectedStyle={styles.selectedStyle}
    />
  );
};

export default MultiSelectPicker;

const styles = StyleSheet.create({
  dropdown: {
    height: 50,
    borderWidth: 1,
    borderRadius: 4,
    padding: 15,
  },
  selectedStyle: {
    borderRadius: 12,
  },
  placeholderStyle: {
    fontSize: 16,
  },
  inputSearchStyle: {
    height: 40,
    fontSize: 16,
  },
  selectedTextStyle: {
    fontSize: 14,
  },
});
