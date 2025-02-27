import { StyleSheet, View } from "react-native";
import React from "react";
import DepartmentList from "../../../../components/departments/DepartmentList";
import { useRouter } from "expo-router";
import { FAB, Portal } from "react-native-paper";
import { useIsFocused } from "@react-navigation/native";

const departments = () => {
  const router = useRouter();
  const isFocused = useIsFocused();

  return (
    <View style={styles.container}>
      <DepartmentList />
      <Portal>
        {isFocused && (
          <FAB
            icon="plus"
            style={styles.fab}
            onPress={() => {
              router.navigate("(auth)/(admin)/department/-1");
            }}
          />
        )}
      </Portal>
    </View>
  );
};

export default departments;

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  iconButton: {
    marginRight: 10,
  },
  fab: {
    position: "absolute",
    margin: 16,
    right: 0,
    bottom: 40,
  },
});
