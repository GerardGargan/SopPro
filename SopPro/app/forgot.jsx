import { StyleSheet, View } from "react-native";
import React, { useState } from "react";
import { ScrollView } from "react-native-gesture-handler";
import Header from "../components/UI/Header";
import { validateEmail } from "../util/validationHelpers";
import InputErrorMessage from "../components/UI/InputErrorMessage";
import { forgotPassword } from "../util/httpRequests";
import { useMutation } from "@tanstack/react-query";
import Toast from "react-native-toast-message";
import CustomTextInput from "../components/UI/form/CustomTextInput";
import CustomButton from "../components/UI/form/CustomButton";

const forgot = () => {
  // Set up state
  const [email, setEmail] = useState("");
  const [emailError, setEmailError] = useState(false);

  // Mutation for sending the request with the users email
  const { mutate, isPending } = useMutation({
    mutationFn: forgotPassword,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Reset Email link sent",
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

  // Triggers validation and mutation
  function handlePress() {
    setEmailError(false);

    const emailValidator = validateEmail(email);
    if (!emailValidator.isFieldValid) {
      setEmailError(emailValidator.message);
      return;
    }

    mutate(email);
  }
  return (
    <ScrollView style={styles.rootContainer}>
      <View style={styles.formContainer}>
        <Header text="Reset password" textStyle={{ color: "black" }} />
        <CustomTextInput
          style={styles.textInput}
          error={emailError !== false}
          label="Email"
          keyboardType="email-address"
          value={email}
          onChangeText={(value) => setEmail(value)}
        />
        {emailError !== false && (
          <InputErrorMessage>{emailError}</InputErrorMessage>
        )}
        <CustomButton
          mode="contained"
          loading={isPending}
          style={{ marginVertical: 10 }}
          onPress={handlePress}
        >
          Reset Password
        </CustomButton>
      </View>
    </ScrollView>
  );
};

export default forgot;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
  textInput: {
    marginVertical: 2,
  },
  buttonContainer: {
    borderRadius: 0,
    marginVertical: 14,
  },
});
