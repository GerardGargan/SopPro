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

  // Set title based on create or update
  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create Department" : "Update Department",
    });
  });

  // use correct mutation function based on create or update
  const mutationFunction = isCreate ? createDepartment : updateDepartment;

  // Mutation hook for updating/creating
  const { mutate, isPending: isPendingMutate } = useMutation({
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

  // Mutation hook for deleting
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

  // Fetch department
  const { data, isPending, isError, error } = useQuery({
    enabled: isCreate == false,
    queryKey: ["departments", id],
    queryFn: () => fetchDepartment(id),
  });

  // Set up state if data is loaded
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

  // Function which shows the delete confirmation prompt
  function handleShowDeletePrompt() {
    setModalVisisble(true);
  }

  // Function which handles a deletion and dismisses the modal
  function handleDeletion() {
    mutateDelete({ id });
    dismissModal();
  }

  // Function which closes the modal
  function dismissModal() {
    setModalVisisble(false);
  }

  // Show loading spinner if a department is being fetched
  if (!isCreate && isPending) {
    return (
      <View style={styles.loader}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  // Show error message if the request returns an error
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
          loading={isPendingMutate}
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
