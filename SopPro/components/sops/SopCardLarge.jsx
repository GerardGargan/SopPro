import {
  StyleSheet,
  Text,
  View,
  Dimensions,
  Image,
  TouchableOpacity,
} from "react-native";
import React from "react";
import { Camera } from "lucide-react-native";

const SopCardLarge = ({ sop, handleOpenBottomSheet }) => {
  return (
    <View style={styles.card}>
      <TouchableOpacity onPress={() => handleOpenBottomSheet(sop)}>
        <View style={styles.imageContainer}>
          {sop.imageUrl ? (
            <Image
              source={{ uri: sop.imageUrl }}
              style={styles.image}
              resizeMode="cover"
            />
          ) : (
            <View style={styles.placeholderImage}>
              <Camera color="black" size={30} />
            </View>
          )}
        </View>
        <View style={styles.contentContainer}>
          <Text style={styles.title} numberOfLines={1}>
            {sop.title || "SOP Title"}
          </Text>
          <Text style={styles.description} numberOfLines={1}>
            {sop.description || "Description"}
          </Text>
        </View>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  card: {
    backgroundColor: "#ffffff",
    borderRadius: 12,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
    margin: 4,
    overflow: "hidden",
    width: Dimensions.get("window").width / 3,
  },
  imageContainer: {
    width: "100%",
    height: 100,
    backgroundColor: "#f5f5f5",
  },
  image: {
    width: "100%",
    height: "100%",
  },
  placeholderImage: {
    width: "100%",
    height: "100%",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#e0e0e0",
  },
  contentContainer: {
    padding: 16,
  },
  title: {
    fontSize: 16,
    fontWeight: "600",
    color: "#212121",
    marginBottom: 8,
  },
  department: {
    fontSize: 14,
    color: "#757575",
  },
});

export default SopCardLarge;
