import { Image, StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React from "react";
import { RadioButton } from "react-native-paper";
import SopStatusChip from "./SopStatusChip";
import { Star, Camera } from "lucide-react-native";

const SopCard = ({
  sop,
  toggleSelect,
  selected,
  isSelectedItems,
  openBottomSheet,
}) => {
  function onPress() {
    if (selected || isSelectedItems) {
      return toggleSelect(sop.id);
    }

    openBottomSheet(sop);
  }

  let image = <Camera color="black" size={30} />;

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
        {sop.isFavourite && (
          <View style={styles.favouriteContainer}>
            <Star fill="#FFC107" size={25} />
          </View>
        )}

        <View style={styles.chipContainer}>
          <SopStatusChip status={sop.status} />
        </View>
      </TouchableOpacity>
    </View>
  );
};

export default SopCard;

const styles = StyleSheet.create({
  cardContainer: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    backgroundColor: "white",
    marginBottom: 8,
    paddingVertical: 12,
  },
  pictureContainer: {
    width: 64,
    height: 64,
    backgroundColor: "lightgrey",
    justifyContent: "center",
    alignItems: "center",
    borderRadius: 10,
    overflow: "hidden",
    marginLeft: 12,
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
  chipContainer: {
    justifyContent: "start",
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
  favouriteContainer: {
    justifyContent: "start",
    alignItems: "center",
  },
});
