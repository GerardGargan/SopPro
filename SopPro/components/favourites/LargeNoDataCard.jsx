import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import { Bookmark } from "lucide-react-native";
import { useRouter } from "expo-router";

const LargeNoDataCard = ({ dataName, text, callbackRoute, buttonText }) => {
  const router = useRouter();

  return (
    <View style={styles.container}>
      <View style={styles.card}>
        <View style={styles.iconContainer}>
          <Bookmark size={24} color="#2563EB" />
        </View>

        <Text style={styles.title}>No {dataName} yet</Text>

        <Text style={styles.description}>{text}</Text>

        <TouchableOpacity
          style={styles.button}
          onPress={() => router.navigate(callbackRoute)}
        >
          <Text style={styles.buttonText}>{buttonText}</Text>
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

export default LargeNoDataCard;
