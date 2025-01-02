import React from "react";
import { StyleSheet } from "react-native";
import { Searchbar } from "react-native-paper";

const SearchInput = ({ value, onChangeText }) => {
  return (
    <Searchbar
      style={styles.searchBar}
      placeholder="Search"
      onChangeText={onChangeText}
      value={value}
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
