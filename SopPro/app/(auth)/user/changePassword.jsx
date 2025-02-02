import { ScrollView, StyleSheet, Text, View } from "react-native";
import React, { useState } from "react";
import { Button, TextInput } from "react-native-paper";
import Toast from "react-native-toast-message";

const changePassword = () => {
  const [currentPassword, setCurrentPassword] = useState("");
  const [isCurrentPasswordVisible, setIsCurrentPasswordVisible] =
    useState(false);

  const [newPassword, setNewPassword] = useState("");
  const [isNewPasswordVisible, setIsNewPasswordVisible] = useState(false);

  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [isConfirmNewPasswordVisible, setIsConfirmNewPasswordVisible] =
    useState(false);

  function handlePasswordChange() {
    const errors = [];

    if (
      !currentPassword ||
      currentPassword.trim() == "" ||
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

    if (errors.length > 0) {
      Toast.show({
        type: "error",
        text1: errors.join("\n"),
        visibilityTime: 3000,
      });
      return;
    }

    console.log("valid.. proceed to send request");
  }

  return (
    <ScrollView style={styles.rootContainer}>
      <Text>Change your password</Text>
      <TextInput
        style={styles.input}
        label="Current password"
        value={currentPassword}
        onChangeText={(value) => setCurrentPassword(value)}
        secureTextEntry={!isCurrentPasswordVisible}
        right={
          <TextInput.Icon
            icon={isCurrentPasswordVisible ? "eye-off" : "eye"}
            onPress={() =>
              setIsCurrentPasswordVisible(!isCurrentPasswordVisible)
            }
          />
        }
      />
      <TextInput
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
      <TextInput
        style={styles.input}
        label="Confirm new password"
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
      <Button
        icon="login"
        mode="contained"
        contentStyle={{ height: 50 }}
        labelStyle={{ fontSize: 20 }}
        style={{ borderRadius: 0, marginVertical: 10 }}
        onPress={handlePasswordChange}
      >
        Update password
      </Button>
    </ScrollView>
  );
};

export default changePassword;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    padding: 20,
  },
  input: {
    marginVertical: 4,
  },
});
