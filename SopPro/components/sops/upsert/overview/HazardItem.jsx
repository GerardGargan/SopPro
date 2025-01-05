import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { IconButton } from "react-native-paper";

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
      <View>
        <IconButton icon="pencil" onPress={() => onEdit(hazard.id)} />
      </View>
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
    backgroundColor: "#f5f5f5",
    borderRadius: 8,
    elevation: 4,
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
});
