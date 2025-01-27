import { StyleSheet, Text, View } from "react-native";
import React from "react";
import CustomChip from "../UI/CustomChip";
import { getStatus } from "../../util/statusHelper";

const SopStatusChip = ({ status }) => {
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
    <CustomChip style={chipStyle}>
      <Text style={[styles.text, { color: chipStyle.textColor }]}>
        {getStatus(status)}
      </Text>
    </CustomChip>
  );
};

export default SopStatusChip;

const styles = StyleSheet.create({
  text: {
    fontSize: 13,
    fontWeight: "400",
    textTransform: "capitalize",
  },
});
