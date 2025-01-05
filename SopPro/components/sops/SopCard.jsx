import { Image, StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React from "react";
import { Icon, RadioButton } from "react-native-paper";
import { Ionicons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import SopStatusChip from "./SopStatusChip";

const SopCard = ({ sop, toggleSelect, selected, isSelectedItems }) => {
  const router = useRouter();
  function onPress() {
    if (selected || isSelectedItems) {
      return toggleSelect(sop.id);
    }

    router.push({
      pathname: "/(auth)/upsert/[id]",
      params: {
        id: sop.id,
      },
    });
  }

  let image = <Icon source="camera" size={25} />;

  if (sop.imageUrl) {
    image = (
      <Image
        source={{ uri: sop.imageUrl }}
        style={styles.image}
        resizeMode="cover"
      />
    );
  }

  return (
    <View style={styles.cardContainer}>
      {selected && (
        <View style={styles.radioButton}>
          <RadioButton status="checked" onPress={() => toggleSelect(sop.id)} />
        </View>
      )}
      <View style={styles.pictureContainer}>{image}</View>
      <TouchableOpacity
        style={{ flexDirection: "row", flex: 1, justifyContent: "flex-start" }}
        onPress={onPress}
        onLongPress={() => toggleSelect(sop.id)}
      >
        <View style={styles.textContainer}>
          <Text style={styles.refText} numberOfLines={1}>
            Ref: {sop.reference} - V{sop.version}
          </Text>
          <Text style={styles.titleText} numberOfLines={1}>
            {sop.title}
          </Text>
          <Text numberOfLines={1}>{sop.description}</Text>
        </View>
        <View style={styles.chipContainer}>
          <SopStatusChip status={sop.status} />
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
    backgroundColor: "white",
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
  chipContainer: {
    justifyContent: "center",
    alignItems: "center",
    marginHorizontal: 10,
  },
  image: {
    width: 75,
    height: 75,
  },
  refText: {
    color: "grey",
    fontSize: 11,
    fontStyle: "italic",
  },
  radioButton: {
    justifyContent: "center",
    alignItems: "center",
  },
});
