import { ScrollView, StyleSheet, Text, View } from "react-native";
import React from "react";
import { ActivityIndicator, Button, Modal, Portal } from "react-native-paper";
import VersionCard from "../VersionCard";
import ErrorBlock from "../../UI/ErrorBlock";

// Reusable component (Modal) which displays a list of SOP versions which a user can select, triggering a function which is passed in from the parent
const VersionPickerModal = ({
  sopVersions,
  title,
  subtitle,
  visible,
  setVisibility,
  onPress,
  isSuccessful,
  isError,
  successMessage,
  errorMessage,
  isDownloading,
}) => {
  const successText = <Text style={styles.successText}>{successMessage}</Text>;

  const errorText = (
    <ErrorBlock>
      <Text>{errorMessage}</Text>
    </ErrorBlock>
  );

  return (
    <Portal>
      <Modal
        visible={visible}
        contentContainerStyle={styles.modalContainer}
        onDismiss={() => {
          setVisibility(false);
        }}
      >
        <View style={styles.exportContainer}>
          {isDownloading && <ActivityIndicator />}
          {isError && errorText}
          {isSuccessful && successText}
          <Text style={styles.title}>{title}</Text>
          <Text style={styles.subtitle}>{subtitle}</Text>

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
                  handleVersionSelect={onPress}
                />
              );
            })}
          </ScrollView>
        </View>
      </Modal>
    </Portal>
  );
};

export default VersionPickerModal;

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
  title: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 14,
    color: "#666",
    marginBottom: 16,
  },
  successText: {
    fontSize: 20,
    marginBottom: 8,
  },
});
