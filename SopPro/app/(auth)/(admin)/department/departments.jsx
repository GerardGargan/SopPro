import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React, { useLayoutEffect } from "react";
import DepartmentList from "../../../../components/departments/DepartmentList";
import { useNavigation, useRouter } from "expo-router";
import { Button, FAB, Portal, useTheme } from "react-native-paper";
import { PlusCircle } from "lucide-react-native";
import { useIsFocused } from "@react-navigation/native";

const departments = () => {
  const navigation = useNavigation();
  const router = useRouter();
  const theme = useTheme();
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
