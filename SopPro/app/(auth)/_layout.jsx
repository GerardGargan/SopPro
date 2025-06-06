import "react-native-reanimated";
import * as SplashScreen from "expo-splash-screen";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useFonts } from "expo-font";

import { useEffect, useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigation } from "expo-router";
import { authActions } from "../../store/authSlice";

import { Stack, Redirect } from "expo-router";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import { StyleSheet, View } from "react-native";
import { ActivityIndicator } from "react-native-paper";
import { useQueryClient } from "@tanstack/react-query";

export {
  // Catch any errors thrown by the Layout component.
  ErrorBoundary,
} from "expo-router";

// Prevent the splash screen from auto-hiding before asset loading is complete.
SplashScreen.preventAutoHideAsync();

export default function RootLayout() {
  const navigator = useNavigation();
  const dispatch = useDispatch();
  const [authChecked, setAuthChecked] = useState(false);
  // Access the redux store to check if the user is logged in
  // This is where we handle authentication
  // This file and code is ran when any screen inside the auth folder is mounted
  // Therefore this code will prevent any screens inside the auth folder from being rendered if the user is not logged in, instead they will be redirected
  const isLoggedIn = useSelector((state) => state.auth.isLoggedIn);
  const [loaded, error] = useFonts({
    SpaceMono: require("../../assets/fonts/SpaceMono-Regular.ttf"),
    ...FontAwesome.font,
  });

  const query = useQueryClient();

  // Expo Router uses Error Boundaries to catch errors in the navigation tree.
  useEffect(() => {
    if (error) throw error;
  }, [error]);

  useEffect(() => {
    if (loaded) {
      SplashScreen.hideAsync();
    }
  }, [loaded]);

  useEffect(() => {
    // Function to check if the user has a token stored in AsyncStorage
    const checkAuth = async () => {
      const token = await AsyncStorage.getItem("authToken");
      const refreshToken = await AsyncStorage.getItem("refreshToken");
      const userInfo = await AsyncStorage.getItem("userInfo");
      const payload = {
        token,
        refreshToken,
        userInfo: JSON.parse(userInfo),
      };

      // Initialise the redux store with the payload
      dispatch(authActions.initialiseAuth(payload));
      setAuthChecked(true);
    };
    checkAuth();
  }, [dispatch]);

  // Load spinner while auth is being checked
  if (!authChecked) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  // User is not logged in, redirect to the home screen
  if (!isLoggedIn) {
    query.clear();
    return <Redirect href="/home" />;
  }

  if (!loaded) {
    return null;
  }

  // User is logged in and no errors - safe to render screens.
  // Set up navigation stack with screens
  return (
    <Stack>
      <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
      <Stack.Screen name="(admin)" options={{ headerShown: false }} />
      <Stack.Screen name="upsert/[id]" options={{ title: "Create SOP" }} />
      <Stack.Screen name="upsert/ai" options={{ title: "Generate with AI" }} />
      <Stack.Screen
        name="user/changePassword"
        options={{ title: "Change Password" }}
      />
    </Stack>
  );
}

const styles = StyleSheet.create({
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
});
