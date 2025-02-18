import { StyleSheet, Text, View } from "react-native";
import React, { useLayoutEffect } from "react";
import { useLocalSearchParams, useNavigation } from "expo-router";
import { fetchDepartment } from "../../../../util/httpRequests";
import { useQuery } from "@tanstack/react-query";

const Upsert = () => {
  const navigation = useNavigation();
  const { id } = useLocalSearchParams();

  useLayoutEffect(() => {
    navigation.setOptions({
      title: id == -1 ? "Create" : "Update",
    });
  });

  const { data, isFetching, isError, error } = useQuery({
    enabled: id != -1,
    queryKey: ["departments", id],
    queryFn: () => fetchDepartment(id),
  });

  if (isFetching) {
    return <Text>Loading...</Text>;
  }

  if (isError) {
    return <Text>{error.message}</Text>;
  }

  return (
    <View>
      <Text>
        {id} {data?.name}
      </Text>
    </View>
  );
};

export default Upsert;

const styles = StyleSheet.create({});
