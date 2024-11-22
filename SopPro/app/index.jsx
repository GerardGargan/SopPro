import { View, Text, StyleSheet, ImageBackground } from "react-native";
import React from "react";
import Header from "../components/UI/Header";
import WelcomeButton from "../components/UI/WelcomeButton";
import { useRouter } from "expo-router";
import { SafeAreaView } from "react-native-safe-area-context";

const App = () => {
  const router = useRouter();
  return (
    <>
      <ImageBackground
        source={require("../assets/images/homeBackground.png")}
        resizeMode="cover"
        style={styles.background}
      >
        <SafeAreaView style={styles.rootContainer}>
          <Header text="Simplify SOP Creation" />
          <Header text="Transform How You Work" />
          <View style={styles.buttonsContainer}>
            <View style={styles.buttonContainer}>
              <WelcomeButton onPress={() => router.push("/login")}>
                <Text>Log in</Text>
              </WelcomeButton>
            </View>

            <View style={styles.buttonContainer}>
              <WelcomeButton onPress={() => router.push("/register")}>
                <Text>Sign up for free!</Text>
              </WelcomeButton>
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
  },
  buttonsContainer: {
    flex: 1,
    marginTop: 70,
  },
  buttonContainer: {
    marginVertical: 10,
  },
});

export default App;
