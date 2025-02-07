import { StyleSheet, Text, View } from "react-native";
import { useLocalSearchParams } from "expo-router";
import React from "react";

const reset = () => {
  const { token, email } = useLocalSearchParams();
  console.log(token, email);
  return (
    <View>
      <Text>{token}</Text>
      <Text>{email}</Text>
    </View>
  );
};

export default reset;

const styles = StyleSheet.create({});
