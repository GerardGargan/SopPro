import React, { useState, useEffect } from "react";
import { FlatList, StyleSheet, Text, View } from "react-native";
import { useQuery } from "@tanstack/react-query";
import { ActivityIndicator } from "react-native-paper";
import { useIsFocused } from "@react-navigation/native";
import Fab from "../../../components/sops/fab";
import SopCard from "../../../components/sops/SopCard";
import { fetchSops } from "../../../util/httpRequests";
import SearchInput from "../../../components/UI/SearchInput.jsx";
import SopList from "../../../components/sops/SopList.jsx";

const Sops = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [debouncedSearchQuery, setDebouncedSearchQuery] = useState("");
  const statusFilter = 1;

  const isFocused = useIsFocused();

  useEffect(() => {
    const handler = setTimeout(() => setDebouncedSearchQuery(searchQuery), 500);
    return () => clearTimeout(handler);
  }, [searchQuery]);

  return (
    <View style={styles.container}>
      <SearchInput value={searchQuery} onChangeText={setSearchQuery} />
      <SopList
        debouncedSearchQuery={debouncedSearchQuery}
        statusFilter={statusFilter}
        searchQuery={searchQuery}
      />
      {isFocused && <Fab />}
    </View>
  );
};

export default Sops;

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
});
