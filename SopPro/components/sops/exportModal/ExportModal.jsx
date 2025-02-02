import { ScrollView, StyleSheet, Text, View } from "react-native";
import React, { useState } from "react";
import { ActivityIndicator, Button, Modal, Portal } from "react-native-paper";
import VersionCard from "./VersionCard";
import { downloadSopVersion } from "../../../util/downloadHelper";
import ErrorBlock from "../../UI/ErrorBlock";

const ExportModal = ({ sopVersions, visible, setVisibility }) => {
  const [isDownloading, setIsDownloading] = useState(false);
  const [isSuccessful, setIsSuccessful] = useState(false);
  const [isError, setIsError] = useState(false);

  async function handleDownload(versionId, reference, title) {
    setIsDownloading(true);
    setIsSuccessful(false);
    setIsError(false);
    try {
      await downloadSopVersion(versionId, reference, title);
      setIsSuccessful(true);
    } catch (e) {
      setIsError(true);
    }
    setIsDownloading(false);
  }

  const successMessage = (
    <Text style={styles.successText}>Download successful! ✅</Text>
  );

  const errorMessage = (
    <ErrorBlock>
      <Text>An error occured, download failed.</Text>
    </ErrorBlock>
  );

  return (
    <Portal>
      <Modal
        visible={visible}
        contentContainerStyle={styles.modalContainer}
        onDismiss={() => setVisibility(false)}
      >
        <View style={styles.exportContainer}>
          {isDownloading && <ActivityIndicator />}
          {isError && errorMessage}
          {isSuccessful && successMessage}
          <Text style={styles.exportTitle}>Export to PDF</Text>
          <Text style={styles.exportSubtitle}>Select a version to export</Text>

          <ScrollView style={styles.versionsContainer}>
            {sopVersions?.map((version) => {
              return (
                <VersionCard
                  key={version.id}
                  id={version.id}
                  versionNumber={version.version}
                  createDate={version.createDate}
                  status={version.status}
                  title={version.title}
                  handleVersionSelect={handleDownload}
                />
              );
            })}
          </ScrollView>
        </View>
      </Modal>
    </Portal>
  );
};

export default ExportModal;

const styles = StyleSheet.create({
  modalContainer: {
    backgroundColor: "white",
    margin: 20,
    borderRadius: 8,
    padding: 20,
  },
  exportContainer: {
    height: "80%",
  },
  versionsContainer: {
    flex: 1,
  },
  exportTitle: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 8,
  },
  exportSubtitle: {
    fontSize: 14,
    color: "#666",
    marginBottom: 16,
  },
  successText: {
    fontSize: 20,
    marginBottom: 8,
  },
});
