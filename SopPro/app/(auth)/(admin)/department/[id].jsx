import { StyleSheet, Text, View } from "react-native";
import React, { useEffect, useLayoutEffect, useState } from "react";
import { useLocalSearchParams, useNavigation, useRouter } from "expo-router";
import { fetchDepartment } from "../../../../util/httpRequests";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ActivityIndicator, Button, TextInput } from "react-native-paper";
import InputErrorMessage from "../../../../components/UI/InputErrorMessage";
import ErrorBlock from "../../../../components/UI/ErrorBlock";
import {
  createDepartment,
  updateDepartment,
} from "../../../../util/httpRequests";
import Toast from "react-native-toast-message";

const Upsert = () => {
  const navigation = useNavigation();
  const { id } = useLocalSearchParams();
  const queryClient = useQueryClient();
  const router = useRouter();
  const isCreate = id == -1;

  const [name, setName] = useState("");
  const [nameError, setNameError] = useState(null);

  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create Department" : "Update Department",
    });
  });

  const mutationFunction = isCreate ? createDepartment : updateDepartment;

  const { mutate } = useMutation({
    mutationFn: mutationFunction,
    onSuccess: () => {
      console.log("success");
      Toast.show({
        type: "success",
        text1: "Success",
        text2: `Department ${isCreate ? "Creaed" : "Updated"}`,
        visibilityTime: 5000,
      });
      queryClient.invalidateQueries("departments");
      router.back();
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: "Oops something went wrong!",
        text2: error.message || "The department was not updated",
        visibilityTime: 5000,
      });
    },
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
      mutate({ name });
    } else {
      mutate({ name, id });
    }
  }

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
  loader: {
    flex: 1,
    justifyContent: "center",
  },
  errorContainer: {
    marginHorizontal: 20,
  },
});
