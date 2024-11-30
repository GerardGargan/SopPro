import { View, Text } from "react-native";
import React from "react";
import { SafeAreaView } from "react-native-safe-area-context";

const test = () => {
  return (
    <View>
      <SafeAreaView>
        <Text>Test - second protected route</Text>
      </SafeAreaView>
    </View>
  );
};

export default test;
