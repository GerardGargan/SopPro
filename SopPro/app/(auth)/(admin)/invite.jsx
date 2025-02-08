import { ScrollView, StyleSheet, Text, View } from "react-native";
import React, { useState } from "react";
import { Button, TextInput } from "react-native-paper";
import Header from "../../../components/UI/Header";
import SelectPicker from "../../../components/UI/SelectPicker";
import { Picker } from "@react-native-picker/picker";
import { validateEmail } from "../../../util/validationHelpers";
import InputErrorMessage from "../../../components/UI/InputErrorMessage";
import { inviteUser } from "../../../util/httpRequests";
import { useMutation } from "@tanstack/react-query";

const invite = () => {
  const [email, setEmail] = useState("");
  const [role, setRole] = useState("user");
  const [emailValidationError, setEmailValidationError] = useState(false);

  const { mutate, isPending } = useMutation({
    mutationFn: inviteUser,
    onSuccess: () => {
      console.log("success");
    },
    onError: (error) => {
      console.log(error);
    },
  });

  function handlePress() {
    setEmailValidationError(false);

    const emailValidator = validateEmail(email);
    if (!emailValidator.isFieldValid) {
      setEmailValidationError(emailValidator.message);
      return;
    }

    mutate({ email, role });
  }

  function handleUpdateRole(value) {
    setRole(value);
  }

  return (
    <ScrollView style={styles.rootContainer}>
      <View style={styles.formContainer}>
        <Header text="Invite user" textStyle={{ color: "black" }} />
        <TextInput
          style={styles.textInput}
          label="Email"
          keyboardType="email-address"
          value={email}
          onChangeText={(value) => setEmail(value)}
        />
        {emailValidationError && (
          <InputErrorMessage>{emailValidationError}</InputErrorMessage>
        )}
        <SelectPicker
          selectedValue={role}
          onValueChange={(value) => handleUpdateRole(value)}
        >
          <Picker.Item label="Basic user" value={"user"} />
          <Picker.Item label="Administrator" value={"admin"} />
        </SelectPicker>
        <Button
          mode="contained"
          loading={isPending}
          contentStyle={{ height: 50 }}
          labelStyle={{ fontSize: 20 }}
          style={styles.buttonContainer}
          onPress={handlePress}
        >
          Invite user
        </Button>
      </View>
    </ScrollView>
  );
};

export default invite;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
  textInput: {
    marginVertical: 8,
  },
  buttonContainer: {
    borderRadius: 0,
    marginVertical: 14,
  },
});
