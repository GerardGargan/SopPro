import React from "react";
import {
  View,
  Platform,
  StyleSheet,
  ImageBackground,
  KeyboardAvoidingView,
  ScrollView
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import Header from "../components/UI/Header";
import LoginForm from "../components/login/LoginForm";

const Login = () => {
  return (
      <ScrollView
        keyboardShouldPersistTaps="handled"
        contentContainerStyle={styles.scrollViewContent}
      >
        <KeyboardAvoidingView
          behavior={Platform.OS === "ios" ? "padding" : "height"}
          style={{ flex: 1 }}
        >
          <SafeAreaView style={styles.rootContainer}>
            <Header text="Log in" textStyle={{ color: 'black '}} />
            <View style={styles.formContainer}>
              <LoginForm />
            </View>
          </SafeAreaView>
        </KeyboardAvoidingView>
      </ScrollView>
  );
};

export default Login;

const styles = StyleSheet.create({
  scrollViewContent: {
    flexGrow: 1,
  },
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
  formContainer: {
    marginTop: 64,
  },
});
