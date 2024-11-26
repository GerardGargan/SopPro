import React from "react";
import { View, Text, StyleSheet, ImageBackground } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import Header from "../components/UI/Header";
import LoginForm from "../components/login/LoginForm";


const login = () => {
  return (
    <ImageBackground
      source={require("../assets/images/background.png")}
      resizeMode="cover"
      style={styles.background}
    >
      <SafeAreaView style={styles.rootContainer}>
        <Header text="Log in" />
        <View style={styles.formContainer}>
          <LoginForm />
        </View>
      </SafeAreaView>
    </ImageBackground>
  );
};

export default login;

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
    marginTop: 300
  }
})