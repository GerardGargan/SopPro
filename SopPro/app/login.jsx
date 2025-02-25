import React from "react";
import {
  View,
  Platform,
  StyleSheet,
  KeyboardAvoidingView,
  ScrollView,
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import Header from "../components/UI/Header";
import LoginForm from "../components/login/LoginForm";
import ErrorBlock from "../components/UI/ErrorBlock";
import useLogin from "../hooks/useLogin";

const Login = () => {
  const { mutate, isPending, isError, error } = useLogin();

  function handleLogin(data) {
    mutate(data);
  }

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
          <View style={styles.formContainer}>
            <LoginForm
              isPending={isPending}
              onSubmit={handleLogin}
              isError={isError}
              error={error}
            />
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
