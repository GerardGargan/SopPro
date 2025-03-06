import { StyleSheet, View } from "react-native";
import React from "react";
import { useRouter } from "expo-router";
import { FAB, Portal } from "react-native-paper";
import { useIsFocused } from "@react-navigation/native";
import UserList from "../../../../components/users/UserList";

const users = () => {
  const router = useRouter();
  const isFocused = useIsFocused();

  return (
    <View style={styles.container}>
      <UserList />
      <Portal>
        {isFocused && (
          <FAB
            icon="plus"
            style={styles.fab}
            onPress={() => {
              router.navigate("(auth)/(admin)/invite");
            }}
          />
        )}
      </Portal>
    </View>
  );
};

export default users;

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
