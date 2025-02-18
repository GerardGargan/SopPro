import { StyleSheet, Text, View } from "react-native";
import React, { useEffect, useLayoutEffect, useState } from "react";
import { useLocalSearchParams, useNavigation } from "expo-router";
import { fetchDepartment } from "../../../../util/httpRequests";
import { useQuery } from "@tanstack/react-query";
import { Button, TextInput } from "react-native-paper";
import InputErrorMessage from "../../../../components/UI/InputErrorMessage";

const Upsert = () => {
  const navigation = useNavigation();
  const { id } = useLocalSearchParams();
  const isCreate = id == -1;

  const [name, setName] = useState("");
  const [nameError, setNameError] = useState(null);

  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create Department" : "Update Department",
    });
  });

  const { data, isFetching, isError, error } = useQuery({
    enabled: id != -1,
    queryKey: ["departments", id],
    queryFn: () => fetchDepartment(id),
  });

  useEffect(() => {
    if (data) {
      setName(data.name);
    }
  }, [data]);

  function handleSubmit() {
    // validate fields
    if (name.trim().length === 0) {
      setNameError("Name field cannnot be empty");
      return;
    }

    // handle creation or update
    if (isCreate) {
      console.log("Create");
    } else {
      console.log("Update");
    }
  }

  if (isFetching) {
    return <Text>Loading...</Text>;
  }

  if (isError) {
    return <Text>{error.message}</Text>;
  }

  return (
    <View style={styles.rootContainer}>
      <Text style={styles.title}>
        {isCreate ? "Create" : "Update"} Department
      </Text>
      <TextInput
        style={styles.input}
        label="Department name"
        value={name}
        onChangeText={(value) => {
          setName(value);
          nameError && setNameError(null);
        }}
        error={nameError}
      />
      {nameError && <InputErrorMessage>{nameError}</InputErrorMessage>}
      <Button
        mode="contained"
        contentStyle={{ height: 50 }}
        labelStyle={{ fontSize: 20 }}
        style={{ borderRadius: 0, marginVertical: 10 }}
        onPress={handleSubmit}
      >
        Save
      </Button>
    </View>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    padding: 20,
  },
  input: {
    marginTop: 8,
  },
  title: {
    fontSize: 20,
    marginBottom: 6,
  },
});
