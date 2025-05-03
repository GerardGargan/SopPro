import { StyleSheet, Text, View } from "react-native";
import React, { useState } from "react";
import HazardItem from "./HazardItem";
import { Button, Modal, Portal, TextInput } from "react-native-paper";
import SelectPicker from "../../../UI/SelectPicker";
import { Picker } from "@react-native-picker/picker";
import { useTheme } from "react-native-paper";
import CustomTextInput from "../../../UI/form/CustomTextInput";
import CustomButton from "../../../UI/form/CustomButton";

// Displays a list of hazard cards
const HazardSection = ({
  hazards,
  selectedHazard,
  setSelectedHazard,
  handleSelectHazard,
  handleAddHazard,
  handleRemoveHazard,
  handleUpdateHazard,
}) => {
  const [showDeleteWarning, setShowDeleteWarning] = useState(false);

  const theme = useTheme();

  // Handles removing a hazard
  function deleteHazard() {
    handleRemoveHazard(selectedHazard);
    setShowDeleteWarning(false);
  }

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
          visible={showDeleteWarning}
          onDismiss={() => setShowDeleteWarning(false)}
          contentContainerStyle={styles.modalContainer}
          dismissable={false}
          dismissableBackButton={false}
        >
          <Text style={styles.warningText}>
            Are you sure you want to delete this?
          </Text>
          <View style={styles.deleteButtonsContainer}>
            <Button onPress={() => setShowDeleteWarning(false)}>No</Button>
            <Button onPress={deleteHazard}>Yes</Button>
          </View>
        </Modal>
      </Portal>
      <Portal>
        <Modal
          visible={selectedHazard !== null && showDeleteWarning === false}
          onDismiss={() => setSelectedHazard(null)}
          contentContainerStyle={styles.modalContainer}
          dismissable={false}
          dismissableBackButton={true}
        >
          <Button
            icon="trash-can"
            onPress={() => setShowDeleteWarning(true)}
            style={{ alignSelf: "flex-end", marginBottom: 10 }}
            mode="outlined"
          >
            Delete
          </Button>
          <CustomTextInput
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
          <CustomTextInput
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
          <Text>Risk level</Text>
          <SelectPicker
            selectedValue={
              hazards.find((hazard) => hazard.key === selectedHazard)
                ?.riskLevel || 1
            }
            onValueChange={(value) =>
              handleUpdateHazard(selectedHazard, "riskLevel", value)
            }
          >
            <Picker.Item label="Low" value={1} />
            <Picker.Item label="Medium" value={2} />
            <Picker.Item label="High" value={3} />
          </SelectPicker>
          <CustomButton onPress={() => setSelectedHazard(null)}>
            Save
          </CustomButton>
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
  deleteButtonsContainer: {
    flexDirection: "row",
    justifyContent: "space-evenly",
  },
  warningText: {
    textAlign: "center",
    fontSize: 18,
    marginBottom: 20,
  },
});
