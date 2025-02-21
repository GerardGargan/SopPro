import { StyleSheet, Text, View } from "react-native";
import React from "react";
import VersionPickerModal from "./VersionPickerModal";
import useDownload from "../../../hooks/useDownload";
import { useMutation } from "@tanstack/react-query";
import { revertSopVersion } from "../../../util/httpRequests";

const RevertModal = ({ sopVersions, visible, setVisibility }) => {
  const { mutate, isSuccess, isPending, isError } = useMutation({
    mutationFn: revertSopVersion,
    onSuccess: () => {},
    onError: (error) => {},
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
