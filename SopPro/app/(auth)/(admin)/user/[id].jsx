import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React, { useEffect, useLayoutEffect, useState } from "react";
import { useLocalSearchParams, useNavigation, useRouter } from "expo-router";
import {
  deleteUser,
  fetchUser,
  updateUser,
} from "../../../../util/httpRequests";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ActivityIndicator, Button, TextInput } from "react-native-paper";
import InputErrorMessage from "../../../../components/UI/InputErrorMessage";
import ErrorBlock from "../../../../components/UI/ErrorBlock";
import Toast from "react-native-toast-message";
import { Trash2 } from "lucide-react-native";
import ConfirmationModal from "../../../../components/UI/ConfirmationModal";
import CustomTextInput from "../../../../components/UI/form/CustomTextInput";
import CustomButton from "../../../../components/UI/form/CustomButton";
import SelectPicker from "../../../../components/UI/SelectPicker";
import { Picker } from "@react-native-picker/picker";

const Upsert = () => {
  const { id } = useLocalSearchParams();
  const queryClient = useQueryClient();
  const router = useRouter();

  const [user, setUser] = useState({
    forname: "",
    surname: "",
    roleName: "",
    email: "",
  });

  const [fieldError, setFieldError] = useState({
    forename: null,
    surname: null,
    roleName: null,
    email: null,
  });

  const [modalVisible, setModalVisisble] = useState(false);

  // Hook for updating a user
  const { mutate: mutateUpdate, isPending: isPendingUpdate } = useMutation({
    mutationFn: () => updateUser(id, user),
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Success",
        text2: `User Updated`,
        visibilityTime: 5000,
      });
      queryClient.invalidateQueries("users");
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: "Oops something went wrong!",
        text2: error.message || "The user was not updated",
        visibilityTime: 5000,
      });
    },
  });

  // Hook for deleting a user
  const { mutate: mutateDelete, isPending: isDeleting } = useMutation({
    mutationFn: deleteUser,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Success",
        text2: `User Deleted`,
        visibilityTime: 5000,
      });
      queryClient.removeQueries({ queryKey: ["users", id] });
      queryClient.invalidateQueries("users");
      router.back();
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: "Oops something went wrong!",
        text2: error.message || "The user was not deleted",
        visibilityTime: 5000,
      });
    },
  });

  // Hook for fetching the user
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["users", id],
    queryFn: () => fetchUser({ id }),
  });

  // Once user is loaded update the state
  useEffect(() => {
    if (data) {
      setUser(data);
    }
  }, [data]);

  // Function which updates the state for the specified field (identifier) and reset validation errors for that field
  function handleInput(identifier, value) {
    setUser((prevState) => {
      return { ...prevState, [identifier]: value };
    });

    setFieldError((prevState) => {
      return { ...prevState, [identifier]: null };
    });
  }

  // Function which resets any errors on specific fields
  function resetErrors() {
    setFieldError({
      forename: null,
      surname: null,
      roleName: null,
      email: null,
    });
  }

  // Triggers validation and submission to update the user
  function handleSubmit() {
    resetErrors();
    // validate fields
    let isError = false;

    if (user.forename.trim().length === 0) {
      setFieldError((prevState) => {
        return { ...prevState, forename: "Forename cant be empty" };
      });
      isError = true;
    }

    if (user.surname.trim().length === 0) {
      setFieldError((prevState) => {
        return { ...prevState, surname: "Surname cant be empty" };
      });
      isError = true;
    }

    // If an error was encountered return so that the request is not sent to the server
    if (isError) {
      return;
    }

    // handle creation or update
    mutateUpdate(id, user);
  }

  // Function which shows the delete confirmation prompt
  function handleShowDeletePrompt() {
    setModalVisisble(true);
  }

  // Function to trigger delete request and close modal
  function handleDeletion() {
    mutateDelete({ id });
    dismissModal();
  }

  // Function to dismiss modal
  function dismissModal() {
    setModalVisisble(false);
  }

  // Show error if request fails
  if (isError) {
    return (
      <View style={styles.errorContainer}>
        <ErrorBlock>
          <Text>{error?.message}</Text>
        </ErrorBlock>
      </View>
    );
  }

  // Show loading spinner
  if (isPending) {
    return (
      <View style={styles.loader}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  const deleteButton = (
    <TouchableOpacity onPress={handleShowDeletePrompt} disabled={isDeleting}>
      <Trash2 size={24} color={false ? "#999" : "#ff4444"} />
    </TouchableOpacity>
  );

  return (
    <>
      <View style={styles.rootContainer}>
        <View style={styles.headerContainer}>
          <Text style={styles.title}>Edit user</Text>
          {deleteButton}
        </View>
        <CustomTextInput
          style={styles.input}
          disabled
          label="Email"
          value={user.email}
        />
        <CustomTextInput
          style={styles.input}
          label="Forename"
          value={user.forename}
          onChangeText={(value) => {
            handleInput("forename", value);
          }}
          error={fieldError.forename}
        />
        {fieldError.forename && (
          <InputErrorMessage>{fieldError.forename}</InputErrorMessage>
        )}
        <CustomTextInput
          style={styles.input}
          label="Surname"
          value={user.surname}
          onChangeText={(value) => {
            handleInput("surname", value);
          }}
          error={fieldError.surname}
        />
        {fieldError.surname && (
          <InputErrorMessage>{fieldError.surname}</InputErrorMessage>
        )}
        <View style={{ paddingTop: 10 }}>
          <SelectPicker
            selectedValue={user.roleName}
            onValueChange={(value) => handleInput("roleName", value)}
          >
            <Picker.Item label="Basic user" value={"user"} />
            <Picker.Item label="Administrator" value={"admin"} />
          </SelectPicker>
        </View>
        <CustomButton
          mode="contained"
          style={{ marginVertical: 10 }}
          onPress={handleSubmit}
          loading={isPendingUpdate}
        >
          Save
        </CustomButton>
      </View>
      <ConfirmationModal
        visible={modalVisible}
        onConfirm={handleDeletion}
        onCancel={dismissModal}
        title="Confirm User Deletion"
        subtitle="This user will be removed from all associated SOPs"
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
