import { FlatList, StyleSheet, Text, View } from "react-native";
import React from "react";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useQuery } from "@tanstack/react-query";
import { ActivityIndicator } from "react-native-paper";
import { fetchSops } from "../../../util/httpRequests";
import SopCard from "../../../components/sops/SopCard";

const sops = () => {
  let searchTerm = "";
  let statusFilter = 1;

  const isFocused = useIsFocused();
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["sops", { searchTerm, statusFilter }],
    queryFn: () => fetchSops({ searchTerm, statusFilter }),
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
      <FlatList
        data={data}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => <SopCard sop={item} />}
        ListFooterComponent={<View style={{ height: 20 }} />}
      />
      {isFocused && <Fab />}
    </View>
  );
};

export default sops;

const styles = StyleSheet.create({
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
});
