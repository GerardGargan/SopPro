import { Dimensions, StyleSheet, View } from "react-native";
import React from "react";
import { SkeletonLoader } from "../UI/Skeleton";

const SopCardLargeSkeleton = () => (
  <View style={styles.card}>
    <View style={styles.imageContainer}>
      <SkeletonLoader height="100%" style={styles.image} />
    </View>
    <View style={styles.contentContainer}>
      <SkeletonLoader height={16} style={styles.title} />
      <SkeletonLoader height={14} style={styles.description} />
    </View>
  </View>
);

export default SopCardLargeSkeleton;

const styles = StyleSheet.create({
  card: {
    backgroundColor: "#ffffff",
    borderRadius: 12,
    shadowColor: "#000",
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
    margin: 4,
    overflow: "hidden",
    width: Dimensions.get("window").width / 3,
    padding: 12,
  },
  imageContainer: {
    width: "100%",
    height: 100,
    backgroundColor: "#f5f5f5",
    borderRadius: 8,
    overflow: "hidden",
  },
  image: {
    width: "100%",
    height: "100%",
  },
  contentContainer: {
    paddingTop: 12,
  },
  title: {
    marginBottom: 8,
    width: "80%",
    borderRadius: 4,
  },
  description: {
    width: "60%",
    borderRadius: 4,
  },
});
