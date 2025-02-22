import { StyleSheet } from "react-native";
import React, { useState } from "react";
import VersionPickerModal from "./VersionPickerModal";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { revertSopVersion } from "../../../util/httpRequests";
import Toast from "react-native-toast-message";
import ConfirmationModal from "../../UI/ConfirmationModal";

const RevertModal = ({ sopVersions, visible, setVisibility }) => {
  const queryClient = useQueryClient();
  const [isConfirmationModalVisible, setIsConfirmationModalVisible] =
    useState(false);
  const [selectedId, setSelectedId] = useState(null);

  const { mutate, isSuccess, isPending, isError } = useMutation({
    mutationFn: revertSopVersion,
    onSuccess: () => {
      queryClient.invalidateQueries(["sops"]);
      setVisibility(false);
      Toast.show({
        type: "success",
        text1: "Success",
        text2: "SOP Successfully reverted",
        visibilityTime: 3000,
      });
    },
    onError: (error) => {
      setVisibility(false);
      Toast.show({
        type: "error",
        text1: "Error",
        text2: error.message,
        visibilityTime: 5000,
      });
    },
  });

  function handleRevert({ id }) {
    setIsConfirmationModalVisible(true);
    setSelectedId(id);
  }

  function handleConfirmRevert() {
    mutate({ versionId: selectedId });
    setIsConfirmationModalVisible(false);
    setSelectedId(null);
  }

  return (
    <>
      <VersionPickerModal
        sopVersions={sopVersions}
        visible={visible}
        setVisibility={setVisibility}
        onPress={handleRevert}
        title="Revert to a previous version"
        subtitle="Select a version to revert to"
        isSuccessful={isSuccess}
        isError={isError}
        isDownloading={isPending}
        successMessage="Success! Reverted to the specified version ✅"
        errorMessage="An error occured"
      />

      <ConfirmationModal
        visible={isConfirmationModalVisible}
        onConfirm={handleConfirmRevert}
        onCancel={() => setIsConfirmationModalVisible(false)}
        title="Confirm version revertion"
        subtitle="This cannot be undone. Any versions after the selected version will be permenantly deleted"
      />
    </>
  );
};

export default RevertModal;

const styles = StyleSheet.create({});
