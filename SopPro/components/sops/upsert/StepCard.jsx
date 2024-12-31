import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React from "react";
import { Icon, RadioButton } from "react-native-paper";
import { Ionicons } from "@expo/vector-icons";

const StepCard = ({
  title,
  text,
  imageUrl,
  selected,
  isAnyItemSelected,
  handleSelect,
  handleStartEdit,
}) => {
  return (
    <TouchableOpacity
      style={styles.container}
      onLongPress={handleSelect}
      onPress={() => {
        if (isAnyItemSelected) {
          return handleSelect();
        }
        return handleStartEdit();
      }}
    >
      {selected && (
        <View style={styles.radioButton}>
          <RadioButton status="checked" />
        </View>
      )}
      <View style={styles.pictureContainer}>
        <Icon source="camera" size={25} />
      </View>
      <View style={styles.textContainer}>
        <Text style={styles.titleText}>{title ? title : "New step"}</Text>
        <Text>{text ? text : "Click to edit details"}</Text>
      </View>
      <View style={styles.iconContainer}>
        <Ionicons name="chevron-forward" size={35} color="grey" />
      </View>
    </TouchableOpacity>
  );
};

export default StepCard;

const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    justifyContent: "space-between",
    padding: 10,
    marginVertical: 8,
    backgroundColor: "white",
    shadowColor: "black",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.26,
    shadowRadius: 8,
    elevation: 5,
  },
  radioButton: {
    justifyContent: "center",
    alignItems: "center",
  },
  pictureContainer: {
    width: 75,
    height: 75,
    backgroundColor: "lightgrey",
    justifyContent: "center",
    alignItems: "center",
  },
  iconContainer: {
    justifyContent: "center",
    alignItems: "center",
  },
  textContainer: {
    padding: 10,
    flex: 1,
    justifyContent: "center",
    textAlign: "left",
  },
  titleText: {
    fontSize: 16,
    fontWeight: "bold",
  },
});
