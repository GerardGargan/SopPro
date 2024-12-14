import { View, Text } from "react-native";
import React, { useEffect } from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import api from '../../api/axiosApi';

const test = () => {

  useEffect(() => {
    //api.get('http://192.168.1.46:5000/WeatherForecast')
  });
  return (
    <View>
      <SafeAreaView>
        <Text>Test - second protected route</Text>
      </SafeAreaView>
    </View>
  );
};

export default test;
