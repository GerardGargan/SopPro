import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React from "react";
import { Pencil } from "lucide-react-native";

// Displays a hazard card with its details and a button to edit it
const HazardItem = ({ hazard, onEdit }) => {
  return (
    <View style={styles.hazardItem}>
      <View style={styles.hazardDetails}>
        <Text style={styles.hazardName} numberOfLines={1}>
          {hazard.name ? hazard.name : "New hazard"}
        </Text>
        <Text style={styles.hazardControl} numberOfLines={1}>
          {hazard.controlMeasure ? hazard.controlMeasure : "Control measure"}
        </Text>
      </View>
      <TouchableOpacity
        onPress={() => onEdit(hazard.id)}
        style={styles.iconButton}
      >
        <Pencil color="black" />
      </TouchableOpacity>
    </View>
  );
};

export default HazardItem;

const styles = StyleSheet.create({
  hazardItem: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    padding: 16,
    marginVertical: 8,
    backgroundColor: "white",
    borderRadius: 8,
    elevation: 2,
  },
  hazardDetails: {
    flex: 1,
  },
  hazardName: {
    fontSize: 16,
    fontWeight: "bold",
  },
  hazardControl: {
    fontSize: 14,
    color: "gray",
  },
  iconButton: {
    padding: 15,
  },
});
