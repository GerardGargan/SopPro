import { StyleSheet, Text, View } from "react-native";
import React from "react";
import VersionPickerModal from "./VersionPickerModal";
import useDownload from "../../../hooks/useDownload";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { revertSopVersion } from "../../../util/httpRequests";
import { QueryClient } from "@tanstack/react-query";
import Toast from "react-native-toast-message";

const RevertModal = ({ sopVersions, visible, setVisibility }) => {
  const queryClient = useQueryClient();
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
      Toast.show({
        type: "error",
        text1: "Error",
        text2: error.message,
        visibilityTime: 5000,
      });
    },
  });

  function handleRevert({ id }) {
    mutate({ versionId: id });
  }

  return (
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
      errorMessage="An error occured, download failed."
    />
  );
};

export default RevertModal;

const styles = StyleSheet.create({});
