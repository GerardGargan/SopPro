import {
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
} from "react-native";
import React, { useState } from "react";
import { useLocalSearchParams } from "expo-router";
import { Button, TextInput } from "react-native-paper";
import { validatePassword } from "../util/validationHelpers";
import { useMutation } from "@tanstack/react-query";
import { completeRegistration } from "../util/httpRequests";
import Toast from "react-native-toast-message";
import InputErrorMessage from "../components/UI/InputErrorMessage";
import Header from "../components/UI/Header";

const registerinvite = () => {
  const { token } = useLocalSearchParams();
  const formattedToken = token.replace(/ /g, "+");

  const [password, setPassword] = useState("");
  const [forename, setForename] = useState("");
  const [surname, setSurname] = useState("");
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);
  const [passwordError, setPasswordError] = useState(null);
  const [forenameError, setForenameError] = useState(null);
  const [surnameError, setSurnameError] = useState(null);

  // Mutation function for completing registration
  const { mutate, isPending } = useMutation({
    mutationFn: completeRegistration,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Registration completed",
        visibilityTime: 3000,
      });
      resetFields();
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  // Validate inputs and send the request
  function handlePress() {
    resetErrors();

    let isError = false;

    // validation
    const passwordValidator = validatePassword(password);
    if (!passwordValidator.isFieldValid) {
      setPasswordError(passwordValidator.message);
      isError = true;
    }

    if (forename.trim().length === 0) {
      setForenameError("Forename cant be empty");
      isError = true;
    }

    if (surname.trim().length === 0) {
      setSurnameError("Surname cant be empty");
      isError = true;
    }

    if (isError) {
      return;
    }

    // send request
    const data = { forename, surname, password, token: formattedToken };
    mutate(data);
  }

  function resetFields() {
    setForename("");
    setSurname("");
    setPassword("");
  }

  function resetErrors() {
    setPasswordError(null);
    setForenameError(null);
    setSurnameError(null);
  }

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      style={styles.container}
      keyboardVerticalOffset={Platform.OS === "ios" ? 50 : 0}
      enabled
    >
      <ScrollView
        contentContainerStyle={styles.scrollContent}
        keyboardShouldPersistTaps="handled"
      >
        <Header text="Complete registration" textStyle={{ color: "black" }} />

        <TextInput
          style={styles.input}
          label="Forename"
          error={forenameError !== null}
          value={forename}
          onChangeText={(value) => setForename(value)}
        />
        {forenameError && (
          <InputErrorMessage>{forenameError}</InputErrorMessage>
        )}
        <TextInput
          style={styles.input}
          label="Surname"
          error={surnameError !== null}
          value={surname}
          onChangeText={(value) => setSurname(value)}
        />
        {surnameError && <InputErrorMessage>{surnameError}</InputErrorMessage>}
        <TextInput
          style={styles.input}
          label="Password"
          error={passwordError !== null}
          value={password}
          onChangeText={(value) => setPassword(value)}
          secureTextEntry={!isPasswordVisible}
          right={
            <TextInput.Icon
              icon={isPasswordVisible ? "eye-off" : "eye"}
              onPress={() => setIsPasswordVisible(!isPasswordVisible)}
            />
          }
        />
        {passwordError && (
          <InputErrorMessage>{passwordError}</InputErrorMessage>
        )}

        <Button
          mode="contained"
          loading={isPending}
          contentStyle={{ height: 50 }}
          labelStyle={{ fontSize: 20 }}
          style={styles.buttonContainer}
          onPress={handlePress}
        >
          Complete registration
        </Button>
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

export default registerinvite;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
  },
  scrollContent: {
    flexGrow: 1,
    margin: 30,
    paddingTop: 50,
    paddingBottom: 120,
  },
  buttonContainer: {
    borderRadius: 0,
    marginVertical: 14,
  },
  input: {
    marginTop: 8,
  },
});
