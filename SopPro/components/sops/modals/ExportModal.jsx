import { StyleSheet } from "react-native";
import React from "react";
import VersionPickerModal from "./VersionPickerModal";
import useDownload from "../../../hooks/useDownload";

const ExportModal = ({ sopVersions, visible, setVisibility }) => {
  // Custom hook for handling downloading files to the phones File System
  const {
    isDownloading,
    isSuccessful: isDownloadSuccessful,
    isError: isDownloadError,
    handleDownload,
  } = useDownload();

  return (
    <VersionPickerModal
      sopVersions={sopVersions}
      visible={visible}
      setVisibility={setVisibility}
      onPress={handleDownload}
      title="Export to PDF"
      subtitle="Select a version to export"
      isSuccessful={isDownloadSuccessful}
      isError={isDownloadError}
      isDownloading={isDownloading}
      successMessage="Download successful! ✅"
      errorMessage="An error occured, download failed."
    />
  );
};

export default ExportModal;

const styles = StyleSheet.create({});
