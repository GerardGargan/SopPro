import { FlatList, StyleSheet, Text, View } from "react-native";
import React from "react";
import { useQuery } from "@tanstack/react-query";
import DepartmentCard from "./DepartmentCard";
import { fetchDepartments } from "../../util/httpRequests";

const DepartmentList = () => {
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["departments"],
    queryFn: fetchDepartments,
  });

  if (isPending) {
    return <Text>Loading</Text>;
  }

  if (isError) {
    return <Text>{error.message}</Text>;
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

const styles = StyleSheet.create({});
