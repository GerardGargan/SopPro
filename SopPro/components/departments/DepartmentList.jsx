import { FlatList, StyleSheet, Text, View } from "react-native";
import React from "react";
import { useQuery } from "@tanstack/react-query";
import DepartmentCard from "./DepartmentCard";
import { fetchDepartments } from "../../util/httpRequests";
import { ActivityIndicator } from "react-native-paper";
import ErrorBlock from "../UI/ErrorBlock";

const DepartmentList = () => {
  const { data, isFetching, isError, error } = useQuery({
    queryKey: ["departments"],
    queryFn: fetchDepartments,
  });

  if (isFetching) {
    return (
      <View style={styles.loader}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  if (isError) {
    return (
      <View style={styles.errorContainer}>
        <ErrorBlock>
          <Text>{error?.message}</Text>
        </ErrorBlock>
      </View>
    );
  }

  return (
    <FlatList
      data={data}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => (
        <DepartmentCard id={item.id} name={item.name} />
      )}
    />
  );
};

export default DepartmentList;

const styles = StyleSheet.create({
  loader: {
    flex: 1,
    justifyContent: "center",
  },
  errorContainer: {
    marginHorizontal: 20,
  },
});
