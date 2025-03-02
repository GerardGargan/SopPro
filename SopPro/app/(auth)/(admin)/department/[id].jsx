import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
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
  deleteDepartment,
} from "../../../../util/httpRequests";
import Toast from "react-native-toast-message";
import { Trash2 } from "lucide-react-native";
import ConfirmationModal from "../../../../components/UI/ConfirmationModal";
import CustomTextInput from "../../../../components/UI/form/CustomTextInput";
import CustomButton from "../../../../components/UI/form/CustomButton";

const Upsert = () => {
  const navigation = useNavigation();
  const { id } = useLocalSearchParams();
  const queryClient = useQueryClient();
  const router = useRouter();
  const isCreate = id == -1;

  const [name, setName] = useState("");
  const [nameError, setNameError] = useState(null);
  const [modalVisible, setModalVisisble] = useState(false);

  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create Department" : "Update Department",
    });
  });

  const mutationFunction = isCreate ? createDepartment : updateDepartment;

  const { mutate, isPending } = useMutation({
    mutationFn: mutationFunction,
    onSuccess: () => {
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

  const { mutate: mutateDelete, isPending: isDeleting } = useMutation({
    mutationFn: deleteDepartment,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Success",
        text2: `Department Deleted`,
        visibilityTime: 5000,
      });
      queryClient.removeQueries({ queryKey: ["departments", id] });
      queryClient.invalidateQueries("departments");
      router.back();
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: "Oops something went wrong!",
        text2: error.message || "The department was not deleted",
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

  function handleShowDeletePrompt() {
    setModalVisisble(true);
  }

  function handleDeletion() {
    mutateDelete({ id });
    dismissModal();
  }

  function dismissModal() {
    setModalVisisble(false);
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

  const deleteButton = (
    <TouchableOpacity onPress={handleShowDeletePrompt} disabled={isDeleting}>
      <Trash2 size={24} color={isDeleting ? "#999" : "#ff4444"} />
    </TouchableOpacity>
  );

  return (
    <>
      <View style={styles.rootContainer}>
        <View style={styles.headerContainer}>
          <Text style={styles.title}>
            {isCreate ? "Create" : "Update"} Department
          </Text>
          {!isCreate && deleteButton}
        </View>
        <CustomTextInput
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
        <CustomButton
          mode="contained"
          style={{ marginVertical: 10 }}
          onPress={handleSubmit}
          loading={isPending}
        >
          Save
        </CustomButton>
      </View>
      <ConfirmationModal
        visible={modalVisible}
        onConfirm={handleDeletion}
        onCancel={dismissModal}
        title="Confirm Department Deletion"
        subtitle="This department will be removed from all associated SOPs"
      />
    </>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    margin: 20,
  },
  input: {
    marginTop: 8,
  },
  title: {
    fontSize: 20,
    marginBottom: 6,
    flex: 1,
  },
  loader: {
    flex: 1,
    justifyContent: "center",
  },
  errorContainer: {
    marginHorizontal: 20,
  },
  headerContainer: {
    flexDirection: "row",
  },
});
