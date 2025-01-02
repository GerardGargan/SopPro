import { StyleSheet, Text, View, FlatList } from "react-native";
import React from "react";
import { useQuery } from "@tanstack/react-query";
import { fetchSops } from "../../util/httpRequests";
import SopCard from "./SopCard";
import { ActivityIndicator } from "react-native-paper";
import ErrorBlock from "../UI/ErrorBlock";

const SopList = ({ debouncedSearchQuery, statusFilter, searchQuery }) => {
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["sops", { debouncedSearchQuery, statusFilter }],
    queryFn: () =>
      fetchSops({ searchQuery: debouncedSearchQuery, statusFilter }),
    enabled: !!debouncedSearchQuery || searchQuery === "",
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
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
        <ErrorBlock>{error.message}</ErrorBlock>
      </View>
    );
  }

  return (
    <>
      {data && data.length === 0 && (
        <Text style={styles.noSopText}>No items found</Text>
      )}
      <FlatList
        data={data}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => <SopCard sop={item} />}
      />
    </>
  );
};

export default SopList;

const styles = StyleSheet.create({
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  noSopText: {
    marginTop: 10,
    textAlign: "center",
    fontSize: 20,
  },
});
