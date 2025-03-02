import { StyleSheet, Modal, ScrollView, Text, View } from "react-native";
import React, { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { uploadImage } from "../../../../util/httpRequests";
import { Button, TextInput } from "react-native-paper";
import ImagePickerComponent from "../../../UI/ImagePicker";
import { Modal as PaperModal } from "react-native-paper";
import { Portal } from "react-native-paper";
import Toast from "react-native-toast-message";
import MultiSelectPicker from "../../../UI/MultiSelectPicker";
import CustomTextInput from "../../../UI/form/CustomTextInput";
import CustomButton from "../../../UI/form/CustomButton";
import ConfirmationModal from "../../../UI/ConfirmationModal";

const EditStep = ({
  visible,
  step,
  handleEditStep,
  handleClose,
  handleSetImageUrl,
  handleDeleteStep,
  ppeList,
  handleEditStepPpe,
}) => {
  const [showDeleteWarning, setShowDeleteWarning] = useState(false);

  const { mutate: uploadMutate, data: imageData } = useMutation({
    mutationFn: ({ formData, key }) => {
      return uploadImage(formData).then((data) => ({ ...data, key }));
    },
    onSuccess: (data) => {
      if (step.key == null) {
        console.log("step key is null");
      }
      handleSetImageUrl(data.key, data.result);
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  function onPpeChange(item) {
    const ppeIds = item.map((i) => i.id);
    handleEditStepPpe(step.key, ppeIds);
  }

  function handleImageUpload(imageUri, key) {
    const formData = new FormData();
    formData.append("file", {
      uri: imageUri,
      name: "image.jpg",
      type: "image/jpeg",
    });

    uploadMutate({ formData, key });
  }

  function deleteStep() {
    handleDeleteStep(step.key);
    setShowDeleteWarning(false);
  }

  return (
    <>
      <ConfirmationModal
        visible={showDeleteWarning}
        onCancel={() => setShowDeleteWarning(false)}
        onConfirm={deleteStep}
        title="Delete step"
        subtitle="Are you sure you want to delete this?"
      />
      {/* <Portal>
        <PaperModal
          visible={showDeleteWarning}
          onDismiss={() => setShowDeleteWarning(false)}
          contentContainerStyle={styles.modalPaperContainer}
          dismissable={false}
          dismissableBackButton={false}
        >
          <Text style={styles.warningText}>
            Are you sure you want to delete this?
          </Text>
          <View style={styles.deleteButtonsContainer}>
            <Button onPress={() => setShowDeleteWarning(false)}>No</Button>
            <Button onPress={deleteStep}>Yes</Button>
          </View>
        </PaperModal>
      </Portal> */}

      <Modal
        visible={visible && !showDeleteWarning}
        onDismiss={handleClose}
        contentContainerStyle={styles.modalContainer}
        onRequestClose={handleClose}
        animationType="slide"
        presentationStyle="fullScreen"
      >
        <ScrollView style={styles.scrollViewContainer}>
          <Button
            icon="trash-can"
            onPress={() => setShowDeleteWarning(true)}
            style={{ alignSelf: "flex-end", marginBottom: 10 }}
            mode="outlined"
          >
            Delete
          </Button>
          <CustomTextInput
            label="Title"
            placeholder="Step title"
            defaultValue={step?.title}
            onChangeText={(value) => handleEditStep(step.key, "title", value)}
          />
          <CustomTextInput
            label="Details"
            placeholder="Enter details or instructions"
            style={[styles.textInput, styles.detailInput]}
            multiline
            numberOfLines={5}
            defaultValue={step?.text}
            onChangeText={(value) => handleEditStep(step.key, "text", value)}
          />
          <MultiSelectPicker
            data={ppeList}
            onChange={(item) => handleEditStepPpe(step.key, item)}
            value={step?.ppeIds}
          />
          <ImagePickerComponent
            imageUrl={step?.imageUrl}
            onSelect={(image) => handleImageUpload(image, step.key)}
          />

          <View style={{ alignItems: "center" }}>
            <CustomButton onPress={handleClose} style={{ width: "80%" }}>
              Save
            </CustomButton>
          </View>
        </ScrollView>
      </Modal>
    </>
  );
};

export default EditStep;

const styles = StyleSheet.create({
  modalContainer: {
    flex: 1,
    justifyContent: "flex-start",
    backgroundColor: "grey",
  },
  keyboardAvoidingView: {
    flex: 1,
    margin: 10,
  },
  scrollViewContainer: {
    padding: 20,
    paddingBottom: 50,
  },
  textInput: {
    marginBottom: 10,
  },
  detailInput: {
    height: 150,
  },
  modalPaperContainer: {
    backgroundColor: "white",
    padding: 20,
    margin: 20,
  },
  deleteButtonsContainer: {
    flexDirection: "row",
    justifyContent: "space-evenly",
  },
  warningText: {
    textAlign: "center",
    fontSize: 18,
    marginBottom: 20,
  },
  container: { padding: 16 },
  iconStyle: {
    width: 20,
    height: 20,
  },
  icon: {
    marginRight: 5,
  },
});
