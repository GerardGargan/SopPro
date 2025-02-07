import { StyleSheet, Text, View } from "react-native";
import { useLocalSearchParams } from "expo-router";
import React, { useState } from "react";
import { Button, TextInput } from "react-native-paper";
import { useMutation } from "@tanstack/react-query";
import { resetPassword } from "../util/httpRequests";
import { validatePassword } from "../util/validationHelpers";
import InputErrorMessage from "../components/UI/InputErrorMessage";
import Toast from "react-native-toast-message";

const reset = () => {
  const { token, email } = useLocalSearchParams();
  const [password, setPassword] = useState("");
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);
  const [passwordError, setPasswordError] = useState(false);

  const formattedToken = token.replace(/ /g, "+");

  const { mutate, isPending } = useMutation({
    mutationFn: resetPassword,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Password reset",
        visibilityTime: 3000,
      });
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  function handlePress() {
    setPasswordError(false);
    const passwordValidator = validatePassword(password);
    if (!passwordValidator.isFieldValid) {
      setPasswordError(passwordValidator.message);
      return;
    }

    mutate({ email, formattedToken, password });
  }

  return (
    <View style={styles.rootContainer}>
      <TextInput
        label="New Password"
        error={passwordError !== false}
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
      {passwordError !== false && (
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
        Reset Password
      </Button>
    </View>
  );
};

export default reset;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
  buttonContainer: {
    borderRadius: 0,
    marginVertical: 14,
  },
});
