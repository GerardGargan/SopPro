import {
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import React from "react";
import { getStatus, getStatusColour } from "../../../util/statusHelper";

const VersionCard = ({
  id,
  versionNumber,
  status,
  createDate,
  title,
  handleVersionSelect,
}) => {
  const dateFormatted = new Date(createDate).toLocaleDateString("en-GB");
  return (
    <TouchableOpacity
      style={styles.versionCard}
      onPress={() => handleVersionSelect(id, versionNumber, title)}
    >
      <View style={styles.versionInfo}>
        <Text style={styles.versionTitle}>Version {versionNumber}</Text>
        <Text style={styles.versionStatus}>{getStatus(status)}</Text>
        <Text style={styles.versionDate}>Created: {dateFormatted}</Text>
      </View>
      <View
        style={[
          styles.statusIndicator,
          {
            backgroundColor: getStatusColour(status),
          },
        ]}
      />
    </TouchableOpacity>
  );
};

export default VersionCard;

const styles = StyleSheet.create({
  versionCard: {
    borderWidth: 1,
    borderColor: "#e5e5e5",
    borderRadius: 8,
    padding: 16,
    marginBottom: 12,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  versionInfo: {
    flex: 1,
  },
  versionTitle: {
    fontSize: 16,
    fontWeight: "500",
    marginBottom: 4,
  },
  versionStatus: {
    fontSize: 14,
    color: "#666",
    marginBottom: 2,
  },
  versionDate: {
    fontSize: 12,
    color: "#888",
  },
  statusIndicator: {
    width: 12,
    height: 12,
    borderRadius: 6,
    marginLeft: 12,
  },
});
