import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { Link } from "expo-router";

const More = () => {
  return (
    <View style={styles.container}>
      <Link href="logout">Logout</Link>
    </View>
  );
};

export default More;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
    paddingTop: 50,
  },
});
