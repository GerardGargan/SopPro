import React, { useState } from "react";
import { StyleSheet, View } from "react-native";
import { TextInput, Button } from "react-native-paper";
import Header from "../UI/Header";

const LoginForm = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);

  return (
    <>
      <Header
        text="Welcome back!"
        textStyle={{ color: "black", textAlign: "left" }}
      />
      <View style={styles.inputContainer}>
        <TextInput
          label="Email"
          value={email}
          onChangeText={(value) => setEmail(value)}
        />
      </View>
      <View style={styles.inputContainer}>
        <TextInput
          label="Password"
          value={password}
          onChangeText={(value) => setPassword(value)}
          secureTextEntry={!isPasswordVisible} // Set this only when rendering, not dynamically
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
            contentStyle={{ height: 50 }}
            labelStyle={{ fontSize: 20 }}
            style={{ borderRadius: 0 }}
            onPress={() => {}}
        >
            Log in
        </Button>
      </View>
    </>
  );
};

export default LoginForm;

const styles = StyleSheet.create({
  inputContainer: {
    marginVertical: 8,
  },
  buttonContainer: {
    marginTop: 20
  }
});