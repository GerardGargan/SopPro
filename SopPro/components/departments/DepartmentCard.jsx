import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React from "react";
import { ChevronRight, CircleDot } from "lucide-react-native";
import { useRouter } from "expo-router";

const DepartmentCard = ({ id, name, onPress }) => {
  const router = useRouter();

  return (
    <TouchableOpacity
      style={styles.cardContainer}
      onPress={() => {
        router.navigate(`(auth)/(admin)/department/${id}`);
      }}
    >
      <CircleDot style={styles.icon} size={25} color="#888" />
      <Text numberOfLines={1} style={styles.text}>
        {name}
      </Text>
      <ChevronRight size={25} color="#888" />
    </TouchableOpacity>
  );
};

export default DepartmentCard;

const styles = StyleSheet.create({
  cardContainer: {
    flexDirection: "row",
    alignItems: "center",
    paddingHorizontal: 16,
    paddingVertical: 16,
    marginVertical: 4,
    width: "100%",
    backgroundColor: "white",
  },
  text: {
    flex: 1,
    fontSize: 18,
    color: "#333",
  },
  icon: {
    marginRight: 16,
    width: 30,
  },
});
