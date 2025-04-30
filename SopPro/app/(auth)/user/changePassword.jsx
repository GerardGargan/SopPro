import { ScrollView, StyleSheet, Text } from "react-native";
import React, { useState } from "react";
import { Button, TextInput } from "react-native-paper";
import Toast from "react-native-toast-message";
import { validatePassword } from "../../../util/validationHelpers";
import { useMutation } from "@tanstack/react-query";
import { changePasswordRequest } from "../../../util/httpRequests";
import CustomTextInput from "../../../components/UI/form/CustomTextInput";
import CustomButton from "../../../components/UI/form/CustomButton";

const ChangePassword = () => {
  // Set up state
  const [oldPassword, setCurrentPassword] = useState("");
  const [isOldPasswordVisible, setIsOldPasswordVisible] = useState(false);

  const [newPassword, setNewPassword] = useState("");
  const [isNewPasswordVisible, setIsNewPasswordVisible] = useState(false);

  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [isConfirmNewPasswordVisible, setIsConfirmNewPasswordVisible] =
    useState(false);

  // Mutation for updating the users password
  const { mutate } = useMutation({
    mutationFn: changePasswordRequest,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Password updated",
        visibilityTime: 3000,
      });
    },
    onError: (error) => {
      console.log(error);
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  // Function triggered when the user presses submit on the form
  // Validate fields and trigger mutation
  function handlePasswordChange() {
    const errors = [];

    if (
      !oldPassword ||
      oldPassword.trim() == "" ||
      !newPassword ||
      newPassword.trim() == "" ||
      !confirmNewPassword ||
      confirmNewPassword.trim() == ""
    ) {
      errors.push(`Password fields can't be blank`);
    }

    if (newPassword !== confirmNewPassword) {
      errors.push(`New passwords don't match!`);
    }

    const passwordValidator = validatePassword(newPassword);
    if (!passwordValidator.isFieldValid) {
      errors.push(passwordValidator.message);
    }

    if (errors.length > 0) {
      Toast.show({
        type: "error",
        text1: errors.join("\n"),
        visibilityTime: 3000,
      });
      return;
    }

    mutate({ oldPassword, newPassword, confirmNewPassword });
  }

  return (
    <ScrollView style={styles.rootContainer}>
      <Text style={styles.title}>Change your password</Text>
      <CustomTextInput
        style={styles.input}
        label="Current password"
        value={oldPassword}
        onChangeText={(value) => setCurrentPassword(value)}
        secureTextEntry={!isOldPasswordVisible}
        right={
          <TextInput.Icon
            icon={isOldPasswordVisible ? "eye-off" : "eye"}
            onPress={() => setIsOldPasswordVisible(!isOldPasswordVisible)}
          />
        }
      />
      <CustomTextInput
        style={styles.input}
        label="New password"
        value={newPassword}
        onChangeText={(value) => setNewPassword(value)}
        secureTextEntry={!isNewPasswordVisible}
        right={
          <TextInput.Icon
            icon={isNewPasswordVisible ? "eye-off" : "eye"}
            onPress={() => setIsNewPasswordVisible(!isNewPasswordVisible)}
          />
        }
      />
      <CustomTextInput
        style={styles.input}
        label="Confirm new password "
        value={confirmNewPassword}
        onChangeText={(value) => setConfirmNewPassword(value)}
        secureTextEntry={!isConfirmNewPasswordVisible}
        right={
          <TextInput.Icon
            icon={isConfirmNewPasswordVisible ? "eye-off" : "eye"}
            onPress={() =>
              setIsConfirmNewPasswordVisible(!isConfirmNewPasswordVisible)
            }
          />
        }
      />
      <CustomButton
        icon="login"
        mode="contained"
        style={{ marginVertical: 10 }}
        onPress={handlePasswordChange}
      >
        Update password
      </CustomButton>
    </ScrollView>
  );
};

export default ChangePassword;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    padding: 20,
  },
  input: {
    marginVertical: 4,
  },
  title: {
    fontSize: 20,
    marginBottom: 4,
  },
});
