import React, { useState } from "react";
import { StyleSheet, View } from "react-native";
import { TextInput, Button } from "react-native-paper";
import Header from "../UI/Header";
import { useRouter } from "expo-router";

const LoginForm = ({ onSubmit, isPending }) => {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);

  function handlePress() {
    onSubmit({ email, password });
  }

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
        <TextInput
          label="Email"
          keyboardType="email-address"
          value={email}
          onChangeText={(value) => setEmail(value)}
        />
      </View>
      <View style={styles.inputContainer}>
        <TextInput
          label="Password"
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
      </View>
      <View style={styles.buttonContainer}>
        <Button
          icon="login"
          mode="contained"
          loading={isPending}
          contentStyle={{ height: 50 }}
          labelStyle={{ fontSize: 20 }}
          style={{ borderRadius: 0 }}
          onPress={handlePress}
        >
          {isPending ? "Loggin in..." : "Log in"}
        </Button>
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
