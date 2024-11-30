import FontAwesome from "@expo/vector-icons/FontAwesome";
import { useFonts } from "expo-font";
import { Stack } from "expo-router";
import * as SplashScreen from "expo-splash-screen";
import { useEffect } from "react";
import { StatusBar } from "expo-status-bar";
import { PaperProvider } from "react-native-paper";
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import "react-native-reanimated";
import { Provider } from "react-redux";
import store from '../store/index';

const queryClient = new QueryClient();

export {
  // Catch any errors thrown by the Layout component.
  ErrorBoundary,
} from "expo-router";

// Prevent the splash screen from auto-hiding before asset loading is complete.
SplashScreen.preventAutoHideAsync();

export default function RootLayout() {

  const [loaded, error] = useFonts({
    SpaceMono: require("../assets/fonts/SpaceMono-Regular.ttf"),
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

  if (!loaded) {
    return null;
  }

  return (
    <Provider store={store}>
      <QueryClientProvider client={queryClient}>
        <PaperProvider>
          <RootLayoutNav />
        </PaperProvider>
      </QueryClientProvider>
    </Provider>
  );
}

function RootLayoutNav() {
  return (
    <>
      <StatusBar style="light" />
      <Stack>
        <Stack.Screen name="index" options={{ headerShown: false }} />
        <Stack.Screen name="login" options={{ title: "Login" }} />
        <Stack.Screen name="register" options={{ title: "Sign up" }} />
        <Stack.Screen name="(auth)" />
      </Stack>
    </>
  );
}
