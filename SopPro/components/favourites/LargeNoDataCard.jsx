import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import { useRouter } from "expo-router";

const LargeNoDataCard = ({
  title,
  text,
  callbackRoute,
  buttonText,
  EmptyIcon,
}) => {
  const router = useRouter();

  // Component card is used as a placeholder for where there is no data.
  // It displays an icon, title, description and a button with a callback to navigate to a specified screen
  return (
    <View style={styles.container}>
      <View style={styles.card}>
        {EmptyIcon && (
          <View style={styles.iconContainer}>
            <EmptyIcon size={24} color="#2563EB" />
          </View>
        )}

        <Text style={styles.title}>{title}</Text>

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
