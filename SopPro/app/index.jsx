import { View, Text, StyleSheet, ImageBackground } from "react-native";
import React from "react";
import Header from "../components/UI/Header";
import { useRouter } from "expo-router";
import { SafeAreaView } from "react-native-safe-area-context";
import { Button } from "react-native-paper";

const App = () => {
  const router = useRouter();
  return (
    <>
      <ImageBackground
        source={require("../assets/images/background.png")}
        resizeMode="cover"
        style={styles.background}
      >
        <SafeAreaView style={styles.rootContainer}>
          <Header text="Simplify SOP Creation" containerStyle={{ marginBottom: 10}} />
          <Header text="Transform How You Work" />
          <View style={styles.buttonsContainer}>
            <View style={styles.buttonContainer}>
              <Button
                icon="login"
                mode="contained"
                contentStyle={{ height: 50 }}
                labelStyle={{ fontSize: 20 }}
                style={{ borderRadius: 0 }}
                onPress={() => router.push('/login')}
              >
                Log in
              </Button>
            </View>

            <View style={styles.buttonContainer}>
            <Button
                icon="account-plus"
                mode="outlined"
                contentStyle={{ height: 50 }}
                labelStyle={{ fontSize: 20 }}
                style={{ borderRadius: 0 }}
                onPress={() => router.push('/register')}
              >
                Sign up your organisation!
              </Button>
            </View>
          </View>
        </SafeAreaView>
      </ImageBackground>
    </>
  );
};

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
  background: {
    flex: 1,
    height: "100%",
  },
  buttonsContainer: {
    flex: 1,
    marginTop: 250,
  },
  buttonContainer: {
    marginVertical: 10,
  },
});

export default App;
