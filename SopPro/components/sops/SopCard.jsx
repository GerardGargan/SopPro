import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React from "react";
import { Icon } from "react-native-paper";
import { Ionicons } from "@expo/vector-icons";
import { useRouter } from "expo-router";

const SopCard = ({ sop }) => {
  const router = useRouter();
  function onPress() {
    router.push({
      pathname: "/(auth)/upsert/[id]",
      params: {
        id: sop.id,
      },
    });
  }
  return (
    <View style={styles.cardContainer}>
      <View style={styles.pictureContainer}>
        <Icon source="camera" size={25} />
      </View>
      <TouchableOpacity
        style={{ flexDirection: "row", flex: 1, justifyContent: "flex-start" }}
        onPress={onPress}
      >
        <View style={styles.textContainer}>
          <Text style={styles.titleText}>{sop.title}</Text>
          <Text>{sop.description}</Text>
        </View>
        <View style={styles.iconContainer}>
          <Ionicons name="chevron-forward" size={35} color="grey" />
        </View>
      </TouchableOpacity>
    </View>
  );
};

export default SopCard;

const styles = StyleSheet.create({
  cardContainer: {
    flexDirection: "row",
    justifyContent: "space-between",
    borderTopColor: "lightgrey",
    borderTopWidth: 1,
  },
  pictureContainer: {
    width: 75,
    height: 75,
    backgroundColor: "lightgrey",
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
  iconContainer: {
    justifyContent: "center",
    alignItems: "center",
  },
});
