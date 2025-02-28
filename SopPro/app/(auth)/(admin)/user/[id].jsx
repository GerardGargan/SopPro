import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React, { useEffect, useLayoutEffect, useState } from "react";
import { useLocalSearchParams, useNavigation, useRouter } from "expo-router";
import { fetchDepartment, fetchUser } from "../../../../util/httpRequests";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ActivityIndicator, Button, TextInput } from "react-native-paper";
import InputErrorMessage from "../../../../components/UI/InputErrorMessage";
import ErrorBlock from "../../../../components/UI/ErrorBlock";
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

  const [user, setUser] = useState(null);
  const [modalVisible, setModalVisisble] = useState(false);

  const { data, isFetching, isError, error } = useQuery({
    queryKey: ["users", id],
    queryFn: () => fetchUser({ id }),
  });

  useEffect(() => {
    if (data) {
      setUser(data);
    }
  }, [data]);

  function handleSubmit() {
    // validate fields
    // handle creation or update
  }

  function handleShowDeletePrompt() {
    setModalVisisble(true);
  }

  function handleDeletion() {
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
    <TouchableOpacity onPress={handleShowDeletePrompt} disabled={false}>
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
          label="Forename"
          value={user.forename}
          onChangeText={(value) => {}}
        />
        <CustomTextInput
          style={styles.input}
          label="Surname"
          value={user.surname}
          onChangeText={(value) => {}}
        />
        <CustomButton
          mode="contained"
          style={{ marginVertical: 10 }}
          onPress={handleSubmit}
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
