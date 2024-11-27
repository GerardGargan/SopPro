import React from "react";
import {
  View,
  StyleSheet,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
  Text
} from "react-native";
import { useState } from "react";
import { SafeAreaView } from "react-native-safe-area-context";
import RegisterForm from "../components/register/RegisterForm";
import { useMutation } from "@tanstack/react-query";
import { registerCompany } from "../util/httpRequests";
import { useRouter } from "expo-router";
import InputErrorMessage from "../components/UI/InputErrorMessage";

const register = () => {
  const [successMessage, setSuccessMessage] = useState(null);
  const router = useRouter();

  const { mutate, isPending, isError, error } = useMutation({
    mutationFn: registerCompany,
    onSuccess: async () => {
      setSuccessMessage("User successfully created");
      setTimeout(() => {
        router.replace("/login");
      }, 3000);
    },
  });

  function handleRegister(formData) {
    const data = { ...formData, organisationName: formData.company };
    delete data.company;

    mutate(data);
  }

  let content;

  content = <RegisterForm onSubmit={handleRegister} isPending={isPending} />;

  if (successMessage) {
    content = <Text>{successMessage}</Text>;
  }

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
