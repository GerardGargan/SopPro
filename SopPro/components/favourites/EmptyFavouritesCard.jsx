import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import { Bookmark } from "lucide-react-native";
import { useRouter } from "expo-router";

const EmptyFavoritesCard = () => {
  const router = useRouter();

  return (
    <View style={styles.container}>
      <View style={styles.card}>
        <View style={styles.iconContainer}>
          <Bookmark size={24} color="#2563EB" />
        </View>

        <Text style={styles.title}>No favourites yet</Text>

        <Text style={styles.description}>
          Add your most-used SOPs to Favourites for quick access. They'll appear
          right here on your home screen.
        </Text>

        <TouchableOpacity
          style={styles.button}
          onPress={() => router.navigate("sops")}
        >
          <Text style={styles.buttonText}>Browse SOPs</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 16,
    width: "100%",
  },
  card: {
    backgroundColor: "#F9FAFB",
    borderRadius: 12,
    padding: 24,
    alignItems: "center",
  },
  iconContainer: {
    backgroundColor: "#DBEAFE",
    padding: 12,
    borderRadius: 100,
    marginBottom: 16,
  },
  title: {
    fontSize: 18,
    fontWeight: "600",
    color: "#111827",
    marginBottom: 8,
  },
  description: {
    fontSize: 14,
    color: "#4B5563",
    textAlign: "center",
    marginBottom: 24,
    lineHeight: 20,
  },
  button: {
    backgroundColor: "#2563EB",
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderRadius: 8,
  },
  buttonText: {
    color: "#FFFFFF",
    fontSize: 14,
    fontWeight: "500",
  },
});

export default EmptyFavoritesCard;
