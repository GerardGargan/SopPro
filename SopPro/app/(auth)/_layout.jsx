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
import { FontAwesome5 } from "@expo/vector-icons";
import { StyleSheet, Text, View } from "react-native";
import { ActivityIndicator } from "react-native-paper";

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
  const isLoggedIn = useSelector((state) => state.auth.isLoggedIn);
  const [loaded, error] = useFonts({
    SpaceMono: require("../../assets/fonts/SpaceMono-Regular.ttf"),
    ...FontAwesome.font,
  });

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
    const checkAuth = async () => {
      const token = await AsyncStorage.getItem("authToken");
      const userInfo = await AsyncStorage.getItem("userInfo");
      const payload = {
        token,
        userInfo: JSON.parse(userInfo),
      };
      dispatch(authActions.initialiseAuth(payload));
      setAuthChecked(true);
    };
    checkAuth();
  }, [dispatch]);

  if (!authChecked) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  if (!isLoggedIn) {
    return <Redirect href="/home" />;
  }

  if (!loaded) {
    return null;
  }

  return (
    <Stack>
      <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
      <Stack.Screen name="upsert/[id]" options={{ title: "Create SOP" }} />
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
