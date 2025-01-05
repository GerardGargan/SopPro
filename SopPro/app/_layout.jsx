import FontAwesome from "@expo/vector-icons/FontAwesome";
import { useFonts } from "expo-font";
import { Stack } from "expo-router";
import * as SplashScreen from "expo-splash-screen";
import { useEffect } from "react";
import { StatusBar } from "expo-status-bar";
import { PaperProvider } from "react-native-paper";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import "react-native-reanimated";
import { Provider } from "react-redux";
import store from "../store/index";
import Toast from "../components/UI/Toast";
import { SafeAreaProvider } from "react-native-safe-area-context";

const queryClient = new QueryClient();

export { ErrorBoundary } from "expo-router";

SplashScreen.preventAutoHideAsync();

export default function RootLayout() {
  const [loaded, error] = useFonts({
    SpaceMono: require("../assets/fonts/SpaceMono-Regular.ttf"),
    ...FontAwesome.font,
  });

  useEffect(() => {
    if (error) throw error;
  }, [error]);

  useEffect(() => {
    if (loaded) {
      SplashScreen.hideAsync();
    }
  }, [loaded]);

  if (!loaded) {
    return null;
  }

  return (
    <SafeAreaProvider>
      <Provider store={store}>
        <QueryClientProvider client={queryClient}>
          <PaperProvider>
            <RootLayoutNav />
            <Toast />
          </PaperProvider>
        </QueryClientProvider>
      </Provider>
    </SafeAreaProvider>
  );
}

function RootLayoutNav() {
  return (
    <>
      <StatusBar style="dark" />
      <Stack>
        <Stack.Screen name="(auth)" options={{ headerShown: false }} />
        <Stack.Screen name="home" options={{ headerShown: false }} />
        <Stack.Screen name="login" options={{ title: "Log in" }} />
        <Stack.Screen name="register" options={{ title: "Sign up " }} />
      </Stack>
    </>
  );
}
