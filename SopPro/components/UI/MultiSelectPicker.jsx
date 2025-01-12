import { StyleSheet } from "react-native";
import { MultiSelect } from "react-native-element-dropdown";
import { useTheme } from "react-native-paper";

import React from "react";

const MultiSelectPicker = ({ data, value, onChange }) => {
  const theme = useTheme();
  return (
    <MultiSelect
      style={[
        styles.dropdown,
        {
          backgroundColor: theme.colors.surfaceVariant,
          borderBottomColor: theme.colors.outline,
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
    borderBottomWidth: 1,
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
