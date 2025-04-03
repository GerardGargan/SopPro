import { View, StyleSheet, Text } from "react-native";
import React from "react";
import { useRouter, useSegments } from "expo-router";
import { SafeAreaView } from "react-native-safe-area-context";
import { LinearGradient } from "expo-linear-gradient";
import { StatusBar } from "expo-status-bar";
import { SvgXml } from "react-native-svg";
import CustomButton from "../components/UI/form/CustomButton";
import organizePhotosSvg from "../assets/icons/home-screen-illustration";

const App = () => {
  const router = useRouter();

  return (
    <>
      <StatusBar style="dark" />
      <LinearGradient colors={["#ffffff", "#f7faff"]} style={styles.background}>
        <SafeAreaView style={styles.container}>
          <View style={styles.contentContainer}>
            <View style={styles.headerContainer}>
              <Text style={styles.mainHeader}>Simplify SOPs</Text>
              <Text style={styles.subHeader}>Transform How You Work</Text>
            </View>

            <View style={styles.illustrationContainer}>
              <SvgXml xml={organizePhotosSvg} height="350" />
            </View>

            <View style={styles.buttonsContainer}>
              <CustomButton
                icon="login"
                mode="contained"
                height={56}
                onPress={() => router.push("/login")}
              >
                Log in
              </CustomButton>

              <CustomButton
                icon="account-plus"
                mode="outlined"
                height={56}
                onPress={() => router.push("/register")}
              >
                Sign up your company!
              </CustomButton>

              <Text style={styles.tagline}>
                Create and manage SOPs with ease
              </Text>
            </View>
          </View>
        </SafeAreaView>
      </LinearGradient>
    </>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  background: {
    flex: 1,
  },
  logoContainer: {
    alignItems: "center",
    marginTop: 20,
  },
  contentContainer: {
    flex: 1,
    paddingHorizontal: 24,
    justifyContent: "space-between",
    paddingBottom: 30,
  },
  headerContainer: {
    alignItems: "center",
    marginTop: 20,
  },
  mainHeader: {
    fontSize: 28,
    fontWeight: "700",
    color: "#1e40af",
    textAlign: "center",
    marginBottom: 8,
    letterSpacing: 0.5,
  },
  subHeader: {
    fontSize: 20,
    fontWeight: "500",
    color: "#4b5563",
    textAlign: "center",
    letterSpacing: 0.25,
  },
  illustrationContainer: {
    alignItems: "center",
    justifyContent: "center",
    marginVertical: 10,
  },
  buttonsContainer: {
    width: "100%",
    marginTop: 20,
  },
  tagline: {
    textAlign: "center",
    marginTop: 16,
    color: "#6b7280",
    fontSize: 14,
    fontWeight: "500",
  },
});

export default App;
