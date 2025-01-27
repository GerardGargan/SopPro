import { StyleSheet, Text, View } from "react-native";
import React from "react";

const SopStatusChip = ({ status }) => {
  function getStatus(identifier) {
    switch (identifier) {
      case 1:
        return "Draft";
      case 2:
        return "In Review";
      case 3:
        return "Approved";
      case 4:
        return "Archived";
      case 5:
        return "Rejected";
      default:
        return "Unknown";
    }
  }

  function getChipStyle(identifier) {
    switch (identifier) {
      case 1:
        return {
          backgroundColor: "#e6f7ff",
          borderColor: "#a3d3f7",
          textColor: "#3498db",
        }; // Light blue for Draft
      case 2:
        return {
          backgroundColor: "#FFE8D6",
          borderColor: "#FFB385",
          textColor: "#E67E22",
        }; // Pastel orange for In Review
      case 3:
        return {
          backgroundColor: "#d3f8d3",
          borderColor: "#27ae60",
          textColor: "#2ecc71",
        }; // Pastel green for Approved
      case 4:
        return {
          backgroundColor: "#e8e8e8",
          borderColor: "#cccccc",
          textColor: "#7f8c8d",
        }; // Light gray for Archived
      case 5:
        return {
          backgroundColor: "#f5d7d7",
          borderColor: "#e74c3c",
          textColor: "#c0392b",
        }; // Subtle pink for Rejected
      default:
        return {
          backgroundColor: "#f0f0f0",
          borderColor: "#cccccc",
          textColor: "#7f8c8d",
        }; // Default
    }
  }

  const chipStyle = getChipStyle(status);

  return (
    <View
      style={[
        styles.chip,
        {
          backgroundColor: chipStyle.backgroundColor,
          borderColor: chipStyle.borderColor,
        },
      ]}
    >
      <Text style={[styles.text, { color: chipStyle.textColor }]}>
        {getStatus(status)}
      </Text>
    </View>
  );
};

export default SopStatusChip;

const styles = StyleSheet.create({
  chip: {
    borderWidth: 1,
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 16,
    alignSelf: "flex-start",
  },
  text: {
    fontSize: 13,
    fontWeight: "400",
    textTransform: "capitalize",
  },
});
