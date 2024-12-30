import { StyleSheet, View, Text } from "react-native";
import { TextInput, Modal, Portal, Button } from "react-native-paper";
import React from "react";
import Header from "../../UI/Header";
import HazardItem from "./hazardItem";

const EditOverview = ({
  title,
  description,
  hazards,
  handleTitleChange,
  handleDescriptionChange,
  handleSelectHazard,
  setSelectedHazard,
  handleAddHazard,
  selectedHazard,
  handleUpdateHazard,
  handleRemoveHazard,
}) => {
  return (
    <>
      <TextInput
        style={styles.textInput}
        label="Title"
        placeholder="Enter title"
        value={title}
        onChangeText={(text) => handleTitleChange(text)}
      />
      <TextInput
        style={[styles.textInput, styles.descInput]}
        label="Description"
        placeholder="Enter description"
        multiline
        numberOfLines={10}
        value={description}
        onChangeText={(text) => handleDescriptionChange(text)}
        scrollEnabled={false}
      />
      <Header text="Safety information" textStyle={{ color: "black" }} />
      {hazards.length === 0 && (
        <Text style={{ textAlign: "center" }}>No hazards to show yet</Text>
      )}
      {hazards.map((hazard) => {
        return (
          <HazardItem
            key={hazard.key}
            hazard={hazard}
            onEdit={() => handleSelectHazard(hazard.key)}
          />
        );
      })}
      <Button icon="plus" onPress={handleAddHazard}>
        Add hazard
      </Button>
      <View style={{ height: 100 }} />
      <Portal>
        <Modal
          visible={selectedHazard !== null}
          onDismiss={() => setSelectedHazard(null)}
          contentContainerStyle={styles.modalContainer}
        >
          <Button
            icon="trash-can"
            onPress={() => handleRemoveHazard(selectedHazard)}
            style={{ alignSelf: "flex-end", marginBottom: 10 }}
            mode="contained"
          >
            Delete
          </Button>
          <TextInput
            label="Hazard"
            placeholder="Hazard description"
            style={styles.textInput}
            value={
              hazards.find((hazard) => hazard.key === selectedHazard)?.name
            }
            onChangeText={(text) =>
              handleUpdateHazard(selectedHazard, "name", text)
            }
          />
          <TextInput
            label="Control measure"
            placeholder="Control measure"
            style={[styles.textInput, styles.controlMeasureInput]}
            multiline
            numberOfLines={3}
            value={
              hazards.find((hazard) => hazard.key === selectedHazard)
                ?.controlMeasure
            }
            onChangeText={(text) =>
              handleUpdateHazard(selectedHazard, "controlMeasure", text)
            }
          />
          <Button mode="text" onPress={() => setSelectedHazard(null)}>
            Close
          </Button>
        </Modal>
      </Portal>
    </>
  );
};

export default EditOverview;

const styles = StyleSheet.create({
  textInput: {
    marginBottom: 10,
  },
  modalContainer: {
    backgroundColor: "white",
    padding: 20,
    margin: 20,
  },
  descInput: {
    height: 150,
  },
  controlMeasureInput: {
    height: 100,
  },
});
