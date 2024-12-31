import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { Link } from "expo-router";
import ImagePickerExample from "../../../components/UI/ImagePickerExample";

const More = () => {
  return (
    <View style={styles.container}>
      <Link href="logout">Logout</Link>
      <ImagePickerExample />
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
