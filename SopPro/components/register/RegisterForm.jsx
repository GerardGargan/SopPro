import React, { useState } from "react";
import { StyleSheet, View } from "react-native";
import { TextInput, Button } from "react-native-paper";
import InputErrorMessage from "../UI/InputErrorMessage";
import {
  validateEmail,
  validatePassword,
  validateName,
} from "../../util/validationHelpers";
import CustomTextInput from "../UI/form/CustomTextInput";
import CustomButton from "../UI/form/CustomButton";

const RegisterForm = ({ onSubmit, isPendingRegistration, isPendingLogin }) => {
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);
  const [formData, setFormData] = useState({
    email: "",
    forename: "",
    surname: "",
    company: "",
    password: "",
  });

  const [isValid, setIsValid] = useState({
    email: true,
    forename: true,
    surname: true,
    company: true,
    password: true,
  });

  const [errorMessage, setErrorMessage] = useState({
    email: "",
    forename: "",
    surname: "",
    company: "",
    password: "",
  });

  // Function triggered on each input change to update state and validate it
  function handleInput(identifier, value) {
    setFormData((prevState) => {
      return { ...prevState, [identifier]: value };
    });
    setIsValid((prevState) => {
      return { ...prevState, [identifier]: true };
    });
  }

  function handleSubmit() {
    // Re-validate all fields
    const emailValidation = validateEmail(formData.email);
    const forenameValidation = validateName(formData.forename, "forename");
    const surnameValidation = validateName(formData.surname, "surname");
    const companyValidation = validateName(formData.company, "company");
    const passwordValidation = validatePassword(formData.password);

    const allValid =
      emailValidation.isFieldValid &&
      forenameValidation.isFieldValid &&
      surnameValidation.isFieldValid &&
      companyValidation.isFieldValid &&
      passwordValidation.isFieldValid;

    // Update the state for displaying error messages
    setIsValid({
      email: emailValidation.isFieldValid,
      forename: forenameValidation.isFieldValid,
      surname: surnameValidation.isFieldValid,
      company: companyValidation.isFieldValid,
      password: passwordValidation.isFieldValid,
    });

    setErrorMessage({
      email: emailValidation.message,
      forename: forenameValidation.message,
      surname: surnameValidation.message,
      company: companyValidation.message,
      password: passwordValidation.message,
    });

    // If request is not already in process and data is valid, call outside function to send the request
    if (!isPendingRegistration && allValid) {
      onSubmit(formData);
    }
  }

  // Function which validates input fields, allows for different validation functions to be passed in
  function validateField(identifier, validateFn) {
    const { isFieldValid, message } = validateFn(
      formData[identifier],
      identifier
    );

    setIsValid((prevState) => {
      return { ...prevState, [identifier]: isFieldValid };
    });

    setErrorMessage((prevState) => {
      return { ...prevState, [identifier]: message };
    });
  }

  return (
    <>
      <View style={styles.inputContainer}>
        <CustomTextInput
          label="Email"
          value={formData.email}
          keyboardType="email-address"
          error={!isValid.email}
          erro
          onBlur={() => validateField("email", validateEmail)}
          onChangeText={(value) => handleInput("email", value)}
        />
        {!isValid.email && (
          <InputErrorMessage>{errorMessage.email}</InputErrorMessage>
        )}
      </View>
      <View style={styles.inputContainer}>
        <CustomTextInput
          label="Forename"
          value={formData.forename}
          error={!isValid.forename}
          onBlur={() => validateField("forename", validateName)}
          onChangeText={(value) => handleInput("forename", value)}
        />
        {!isValid.forename && (
          <InputErrorMessage>{errorMessage.forename}</InputErrorMessage>
        )}
      </View>
      <View style={styles.inputContainer}>
        <CustomTextInput
          label="Surname"
          error={!isValid.surname}
          value={formData.surname}
          onBlur={() => validateField("surname", validateName)}
          onChangeText={(value) => handleInput("surname", value)}
        />
        {!isValid.surname && (
          <InputErrorMessage>{errorMessage.surname}</InputErrorMessage>
        )}
      </View>
      <View style={styles.inputContainer}>
        <CustomTextInput
          label="Company name"
          error={!isValid.company}
          value={formData.company}
          onBlur={() => validateField("company", validateName)}
          onChangeText={(value) => handleInput("company", value)}
        />
        {!isValid.company && (
          <InputErrorMessage>{errorMessage.company}</InputErrorMessage>
        )}
      </View>
      <View style={styles.inputContainer}>
        <CustomTextInput
          label="Password"
          autoCorrect={false}
          error={!isValid.password}
          value={formData.password}
          onBlur={() => validateField("password", validatePassword)}
          onChangeText={(value) => handleInput("password", value)}
          secureTextEntry={!isPasswordVisible}
          right={
            <TextInput.Icon
              icon={isPasswordVisible ? "eye-off" : "eye"}
              onPress={() => setIsPasswordVisible(!isPasswordVisible)}
            />
          }
        />
        {!isValid.password && (
          <InputErrorMessage>{errorMessage.password}</InputErrorMessage>
        )}
      </View>
      <View style={styles.buttonContainer}>
        <CustomButton
          mode="contained"
          loading={isPendingRegistration || isPendingLogin}
          height={56}
          onPress={handleSubmit}
        >
          Create account
        </CustomButton>
      </View>
    </>
  );
};

export default RegisterForm;

const styles = StyleSheet.create({
  inputContainer: {
    marginVertical: 8,
  },
  buttonContainer: {
    marginTop: 20,
  },
});
