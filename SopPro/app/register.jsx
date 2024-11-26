import React from "react";
import { View, Text, StyleSheet, ImageBackground } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import Header from "../components/UI/Header";
import RegisterForm from "../components/register/RegisterForm";


const register = () => {
  return (
      <SafeAreaView style={styles.rootContainer}>
        <View style={styles.formContainer}>
          <RegisterForm />
        </View>
      </SafeAreaView>
  );
};

export default register;

const styles = StyleSheet.create({
  background: {
    flex: 1,
    height: '100%'
  },
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
  formContainer: {
  }
})