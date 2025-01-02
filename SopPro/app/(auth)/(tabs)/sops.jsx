import React, { useState, useEffect } from "react";
import { FlatList, StyleSheet, Text, View } from "react-native";
import { useQuery } from "@tanstack/react-query";
import { ActivityIndicator } from "react-native-paper";
import { useIsFocused } from "@react-navigation/native";
import Fab from "../../../components/sops/fab";
import SopCard from "../../../components/sops/SopCard";
import { fetchSops } from "../../../util/httpRequests";
import SearchInput from "../../../components/UI/SearchInput.jsx";

const Sops = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [debouncedSearchQuery, setDebouncedSearchQuery] = useState("");
  const statusFilter = 1;

  const isFocused = useIsFocused();

  useEffect(() => {
    const handler = setTimeout(() => setDebouncedSearchQuery(searchQuery), 500);
    return () => clearTimeout(handler);
  }, [searchQuery]);

  const { data, isPending, isError, error } = useQuery({
    queryKey: ["sops", { debouncedSearchQuery, statusFilter }],
    queryFn: () =>
      fetchSops({ searchQuery: debouncedSearchQuery, statusFilter }),
    enabled: !!debouncedSearchQuery || searchQuery === "",
  });

  if (isPending) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  if (isError) {
    return (
      <View style={styles.centered}>
        <Text>{error.message}</Text>
      </View>
    );
  }

  return (
    <View>
      <SearchInput value={searchQuery} onChangeText={setSearchQuery} />
      <FlatList
        data={data}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => <SopCard sop={item} />}
      />
      {isFocused && <Fab />}
    </View>
  );
};

export default Sops;

const styles = StyleSheet.create({
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
});
