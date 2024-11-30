import React from "react";
import {
  View,
  StyleSheet,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
  Text
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import RegisterForm from "../components/register/RegisterForm";
import { useMutation } from "@tanstack/react-query";
import { registerCompany } from "../util/httpRequests";
import InputErrorMessage from "../components/UI/InputErrorMessage";
import useLogin from "../hooks/useLogin";

const register = () => {
  const { mutate: loginMutate, isPending: isPendingLogin, isError: isLoginError } = useLogin();

  const { mutate, isPending, isError, error } = useMutation({
    mutationFn: registerCompany,
    onSuccess: (responseData, variables) => {
      const { email, password } = variables;
      loginMutate({ email, password });
    },
  });

  function handleRegister(formData) {
    const data = { ...formData, organisationName: formData.company };
    delete data.company;

    mutate(data);
  }

  let content;

  content = <RegisterForm onSubmit={handleRegister} isPendingRegistration={isPending} isPendingLogin={isPendingLogin} />;

  return (
    <ScrollView
      keyboardShouldPersistTaps="handled"
      contentContainerStyle={{ flexGrow: 1 }}
    >
      <SafeAreaView style={styles.rootContainer}>
        <KeyboardAvoidingView
          behavior={Platform.OS === "ios" ? "padding" : "height"}
          style={{ flex: 1 }}
        >
          <View>
            {content}
            {isError && <InputErrorMessage>{error.message}</InputErrorMessage>}
            {isLoginError && <InputErrorMessage>Registration was successful but automatic log in failed. Please try to login manually.</InputErrorMessage>}
          </View>
        </KeyboardAvoidingView>
      </SafeAreaView>
    </ScrollView>
  );
};

export default register;

const styles = StyleSheet.create({
  background: {
    flex: 1,
    height: "100%",
  },
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
});
