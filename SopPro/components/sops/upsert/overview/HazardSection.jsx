import { StyleSheet, Text, View } from "react-native";
import React from "react";
import HazardItem from "./HazardItem";
import { Button, Modal, Portal, TextInput } from "react-native-paper";

const HazardSection = ({
  hazards,
  selectedHazard,
  setSelectedHazard,
  handleSelectHazard,
  handleAddHazard,
  handleRemoveHazard,
  handleUpdateHazard,
}) => {
  return (
    <>
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
          dismissable={false}
          dismissableBackButton={true}
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
            defaultValue={
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
            defaultValue={
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

export default HazardSection;

const styles = StyleSheet.create({
  textInput: {
    marginBottom: 10,
  },
  modalContainer: {
    backgroundColor: "white",
    padding: 20,
    margin: 20,
  },
  controlMeasureInput: {
    height: 100,
  },
});
