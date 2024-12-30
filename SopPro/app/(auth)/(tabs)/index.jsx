import { Text } from "react-native";
import React from "react";
import { useSelector } from "react-redux";
import { SafeAreaView } from "react-native-safe-area-context";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useRouter } from "expo-router";
import { Button } from "react-native-paper";

const index = () => {
  const isFocused = useIsFocused();
  const isLoggedIn = useSelector((state) => state.auth.isLoggedIn);
  const router = useRouter();
  return (
    <SafeAreaView>
      <Text>Logged in, this is a protected route</Text>
      <Button
        onPress={() =>
          router.push({
            pathname: "/(auth)/upsert/[id]",
            params: {
              id: 1003,
            },
          })
        }
      >
        Edit existing SOP
      </Button>
      <Button
        onPress={() =>
          router.push({
            pathname: "/(auth)/upsert/[id]",
            params: {
              id: -1,
            },
          })
        }
      >
        Create new SOP
      </Button>
      {isFocused && <Fab />}
    </SafeAreaView>
  );
};

export default index;
