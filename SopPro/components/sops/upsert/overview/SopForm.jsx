import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { TextInput } from "react-native-paper";
import { Picker } from "@react-native-picker/picker";
import SelectPicker from "../../../UI/SelectPicker";
const SopForm = ({
  selectedDepartment,
  departments,
  handleSelectDepartment,
  handleDescriptionChange,
  handleTitleChange,
  title,
  description,
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
      <SelectPicker
        selectedValue={selectedDepartment}
        onValueChange={handleSelectDepartment}
        mode="dropdown"
      >
        <Picker.Item label="Select department" value={-1} />
        {departments.map((department) => {
          return (
            <Picker.Item
              key={department.id}
              label={department.name}
              value={department.id}
            />
          );
        })}
      </SelectPicker>
    </>
  );
};

export default SopForm;

const styles = StyleSheet.create({
  textInput: {
    marginBottom: 10,
  },
  descInput: {
    height: 150,
  },
});
