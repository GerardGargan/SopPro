import React, { useState } from "react";
import { StyleSheet, View } from "react-native";
import { TextInput, Button } from "react-native-paper";
import Header from "../UI/Header";
import { useRouter } from "expo-router";
import CustomButton from "../UI/form/CustomButton";
import CustomTextInput from "../UI/form/CustomTextInput";
import ErrorBlock from "../UI/ErrorBlock";

const LoginForm = ({ onSubmit, isPending, isError, error }) => {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);

  // Trigger outside function for submitting the form with the data
  function handlePress() {
    onSubmit({ email, password });
  }

  // Function to navigate to the forgot password screen
  function handleForgotPasswordPress() {
    router.navigate("forgot");
  }

  return (
    <>
      <Header
        text="Welcome back!"
        textStyle={{ color: "black", textAlign: "left" }}
        containerStyle={{ alignItems: "start" }}
      />
      <View style={styles.inputContainer}>
        <CustomTextInput
          label="Email"
          keyboardType="email-address"
          value={email}
          onChangeText={(value) => setEmail(value)}
        />
      </View>
      <View style={styles.inputContainer}>
        <CustomTextInput
          label="Password"
          value={password}
          autoCorrect={false}
          onChangeText={(value) => setPassword(value)}
          secureTextEntry={!isPasswordVisible}
          right={
            <TextInput.Icon
              icon={isPasswordVisible ? "eye-off" : "eye"}
              onPress={() => setIsPasswordVisible(!isPasswordVisible)}
            />
          }
        />
      </View>
      {isError && <ErrorBlock>{error.message}</ErrorBlock>}
      <View style={styles.buttonContainer}>
        <CustomButton
          icon="login"
          mode="contained"
          height={56}
          loading={isPending}
          onPress={handlePress}
        >
          {isPending ? "Logging in..." : "Log in"}
        </CustomButton>
      </View>
      <Button
        style={{ marginVertical: 8 }}
        mode="text"
        onPress={handleForgotPasswordPress}
      >
        Forgot Password
      </Button>
    </>
  );
};

export default LoginForm;

const styles = StyleSheet.create({
  inputContainer: {
    marginVertical: 8,
  },
  buttonContainer: {
    marginTop: 20,
  },
});
