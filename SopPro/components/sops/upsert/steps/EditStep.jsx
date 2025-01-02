import { StyleSheet, Modal, ScrollView } from "react-native";
import React from "react";
import { useMutation } from "@tanstack/react-query";
import { uploadImage } from "../../../../util/httpRequests";
import { Button, TextInput } from "react-native-paper";
import ImagePickerComponent from "../../../UI/ImagePicker";

const EditStep = ({
  visible,
  step,
  handleEditStep,
  handleClose,
  handleSetImageUrl,
}) => {
  const {
    mutate: uploadMutate,
    data: imageData,
    isError,
    error,
    isPending,
  } = useMutation({
    mutationFn: uploadImage,
    onSuccess: (data) => {
      console.log("Image uploaded successfully:", data);
      handleSetImageUrl(step.key, data.result);
    },
    onError: (error) => {
      console.error("Error uploading image:", error);
    },
  });

  function handleImageUpload(imageUri) {
    const formData = new FormData();
    formData.append("file", {
      uri: imageUri,
      name: "image.jpg",
      type: "image/jpeg",
    });

    uploadMutate(formData);
  }

  return (
    <Modal
      visible={visible}
      onDismiss={handleClose}
      contentContainerStyle={styles.modalContainer}
      onRequestClose={handleClose}
      animationType="slide"
      presentationStyle="fullScreen"
    >
      <ScrollView style={styles.scrollViewContainer}>
        <TextInput
          label="Title"
          placeholder="Step title"
          style={styles.textInput}
          defaultValue={step?.title}
          onChangeText={(value) => handleEditStep(step.key, "title", value)}
        />
        <TextInput
          label="Details"
          placeholder="Enter details or instructions"
          style={[styles.textInput, styles.detailInput]}
          multiline
          numberOfLines={5}
          defaultValue={step?.text}
          onChangeText={(value) => handleEditStep(step.key, "text", value)}
        />
        <ImagePickerComponent
          imageUrl={step?.imageUrl}
          onSelect={(image) => handleImageUpload(image)}
        />
        <Button mode="text" onPress={handleClose}>
          Close
        </Button>
      </ScrollView>
    </Modal>
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
  },
  textInput: {
    marginBottom: 10,
  },
  detailInput: {
    height: 150,
  },
});
