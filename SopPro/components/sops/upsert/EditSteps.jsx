import { StyleSheet, Text, View } from "react-native";
import React from "react";
import StepCard from "./StepCard";
import {
  Button,
  FAB,
  IconButton,
  Portal,
  Modal,
  TextInput,
} from "react-native-paper";
import { useState } from "react";
import ImagePickerComponent from "../../UI/ImagePicker";
import { uploadImage } from "../../../util/httpRequests";
import { useMutation } from "@tanstack/react-query";

const EditSteps = ({ steps, setSteps }) => {
  const [selectedItem, setSelectedItem] = useState(null);
  const [editItem, setEditItem] = useState(null);

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
    },
    onError: (error) => {
      console.error("Error uploading image:", error);
    },
  });

  function handleEditStep(key, identifier, value) {
    setSteps((prevState) => {
      const index = prevState.findIndex((step) => step.key === key);
      const newSteps = [...prevState];
      newSteps[index][identifier] = value;
      return newSteps;
    });
  }

  function handleAddStep() {
    setSteps((prevState) => {
      const maxKey =
        prevState.length > 0
          ? Math.max(...prevState.map((step) => step.key))
          : 0;
      const maxPosition =
        prevState.length > 0
          ? Math.max(...prevState.map((step) => step.position))
          : 0;

      const newStep = {
        id: null,
        key: maxKey + 1,
        text: "",
        imageUrl: "",
        position: maxPosition + 1,
      };

      return [...prevState, newStep];
    });
  }

  function handlePositionMove(direction) {
    setSteps((prevState) => {
      const index = prevState.findIndex((step) => step.key === selectedItem);

      if (direction === "up" && index > 0) {
        const newSteps = [...prevState];
        const temp = newSteps[index];
        newSteps[index] = newSteps[index - 1];
        newSteps[index - 1] = temp;

        newSteps[index].position += 1;
        newSteps[index - 1].position -= 1;

        return newSteps;
      } else if (direction === "down" && index < prevState.length - 1) {
        const newSteps = [...prevState];
        const temp = newSteps[index];
        newSteps[index] = newSteps[index + 1];
        newSteps[index + 1] = temp;

        newSteps[index].position -= 1;
        newSteps[index + 1].position += 1;

        return newSteps;
      }

      return prevState;
    });
  }

  function handleImageUpload(imageUri) {
    const formData = new FormData();
    formData.append("file", {
      uri: imageUri,
      name: "image.jpg",
      type: "image/jpeg",
    });

    uploadMutate(formData);
  }

  const ReorderSection = () => {
    return (
      <View style={{ flexDirection: "row", justifyContent: "space-between" }}>
        <View style={{ flexDirection: "row", justifyContent: "flex-start" }}>
          <IconButton
            mode="contained-tonal"
            icon="arrow-up-bold"
            size={30}
            onPress={() => {
              handlePositionMove("up");
            }}
          />
          <IconButton
            mode="contained-tonal"
            icon="arrow-down-bold"
            size={30}
            onPress={() => {
              handlePositionMove("down");
            }}
          />
        </View>
        <View>
          <Button
            mode="text"
            icon="check"
            style={{ fontSize: 30 }}
            contentStyle={{ height: 50 }}
            labelStyle={{ fontSize: 15 }}
            onPress={() => setSelectedItem(null)}
          >
            Done
          </Button>
        </View>
      </View>
    );
  };

  return (
    <>
      <View style={{ flex: 1 }}>
        {selectedItem && <ReorderSection />}
        {steps.map((step) => {
          return (
            <StepCard
              key={step.key}
              text={step.text}
              title={step.title}
              imageUrl={step.imageUrl}
              handleSelect={() => setSelectedItem(step.key)}
              handleStartEdit={() => {
                setEditItem(step.key);
              }}
              isAnyItemSelected={selectedItem !== null}
              selected={selectedItem === step.key}
            />
          );
        })}
        <View style={{ height: 100 }} />
      </View>
      <Portal>
        <FAB icon="plus" style={styles.fab} onPress={handleAddStep} />
      </Portal>
      <Portal>
        <Modal
          visible={editItem !== null}
          onDismiss={() => setEditItem(null)}
          contentContainerStyle={styles.modalContainer}
          dismissable={false}
          dismissableBackButton={true}
        >
          <TextInput
            label="Title"
            placeholder="Step title"
            style={styles.textInput}
            defaultValue={steps.find((step) => step.key === editItem)?.title}
            onChangeText={(value) => handleEditStep(editItem, "title", value)}
          />
          <TextInput
            label="Details"
            placeholder="Enter details or instructions"
            style={[styles.textInput, styles.detailInput]}
            multiline
            numberOfLines={5}
            defaultValue={steps.find((step) => step.key === editItem)?.text}
            onChangeText={(value) => handleEditStep(editItem, "text", value)}
          />
          <ImagePickerComponent
            imageUrl={steps.find((step) => step.key == editItem)?.imageUrl}
            onSelect={(image) => handleImageUpload(image)}
          />
          <Button mode="text" onPress={() => setEditItem(null)}>
            Close
          </Button>
        </Modal>
      </Portal>
    </>
  );
};

export default EditSteps;

const styles = StyleSheet.create({
  fab: {
    position: "absolute",
    margin: 16,
    right: 0,
    bottom: 80,
  },
  modalContainer: {
    flex: 1,
    justifyContent: "flex-start",
    backgroundColor: "white",
    borderRadius: 5,
    padding: 20,
    margin: 20,
  },
  textInput: {
    marginBottom: 10,
  },
  detailInput: {
    height: 150,
  },
});
