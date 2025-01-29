import { ScrollView, StyleSheet, Text, View } from "react-native";
import React from "react";
import { Modal, Portal } from "react-native-paper";
import VersionCard from "./VersionCard";

const ExportModal = ({ sopVersions, visible, setVisibility }) => {
  return (
    <Portal>
      <Modal
        visible={visible}
        contentContainerStyle={styles.modalContainer}
        onDismiss={() => setVisibility(false)}
      >
        <View style={styles.exportContainer}>
          <Text style={styles.exportTitle}>Export Modal</Text>
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
                  handleVersionSelect={() => {}}
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
});
