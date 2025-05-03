import React from "react";
import { StyleSheet } from "react-native";
import { Searchbar } from "react-native-paper";

// Search input component
const SearchInput = ({ value, onChangeText, ...props }) => {
  return (
    <Searchbar
      style={styles.searchBar}
      placeholder="Search"
      onChangeText={onChangeText}
      value={value}
      {...props}
    />
  );
};

export default SearchInput;

const styles = StyleSheet.create({
  searchBar: {
    marginHorizontal: 10,
    marginVertical: 10,
  },
});
