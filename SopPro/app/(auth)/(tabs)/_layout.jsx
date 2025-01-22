import { Tabs, useFocusEffect } from "expo-router";
import React, { useCallback, useRef, useState } from "react";
import { FontAwesome5, FontAwesome6 } from "@expo/vector-icons";
import SafeAreaHeader from "../../../components/UI/SareAreaHeader";
import { Button, useTheme } from "react-native-paper";
import BottomSheet, { BottomSheetView } from "@gorhom/bottom-sheet";
import {
  BackHandler,
  StyleSheet,
  Text,
  TouchableWithoutFeedback,
  View,
} from "react-native";

function TabBarIcon({ ...props }) {
  return <FontAwesome5 size={28} style={{ marginBottom: -3 }} {...props} />;
}

const _layout = () => {
  const theme = useTheme();
  const bottomSheetRef = useRef(null);

  const [isSheetOpen, setIsSheetOpen] = useState(true);

  const handleSheetChanges = useCallback((index) => {
    setIsSheetOpen(index >= 0);
  }, []);

  const closeBottomSheet = () => {
    bottomSheetRef.current?.close();
    setIsSheetOpen(false);
  };

  const openBottomSheet = () => {
    bottomSheetRef.current?.expand();
    setIsSheetOpen(true);
  };

  return (
    <>
      <SafeAreaHeader />
      <Button onPress={openBottomSheet}>Open</Button>
      <Tabs
        screenOptions={{
          headerShown: false,
          tabBarStyle: {
            backgroundColor: theme.colors.surface,
            borderTopColor: theme.colors.outline,
          },
          tabBarActiveTintColor: theme.colors.primary,
          tabBarInactiveTintColor: theme.colors.onSurfaceVariant,
        }}
      >
        <Tabs.Screen
          name="index"
          options={{
            title: "Home",
            tabBarIcon: ({ color }) => <TabBarIcon name="home" color={color} />,
          }}
        />
        <Tabs.Screen
          name="sops"
          options={{
            title: "Sops",
            tabBarIcon: ({ color }) => (
              <TabBarIcon name="clipboard-list" color={color} />
            ),
          }}
        />
        <Tabs.Screen
          name="more"
          options={{
            title: "More",
            tabBarIcon: ({ color }) => (
              <TabBarIcon name="th-large" color={color} />
            ),
          }}
        />
      </Tabs>

      {isSheetOpen && (
        <TouchableWithoutFeedback onPress={closeBottomSheet}>
          <View style={styles.overlay} />
        </TouchableWithoutFeedback>
      )}

      <BottomSheet
        ref={bottomSheetRef}
        index={isSheetOpen ? 0 : -1}
        snapPoints={["50%"]}
        onChange={handleSheetChanges}
        enablePanDownToClose={true}
      >
        <BottomSheetView style={styles.contentContainer}>
          <Text>Awesome 🎉</Text>
          <Button onPress={closeBottomSheet}>Close</Button>
        </BottomSheetView>
      </BottomSheet>
    </>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "grey",
  },
  contentContainer: {
    flex: 1,
    padding: 36,
    alignItems: "center",
  },
  overlay: {
    ...StyleSheet.absoluteFillObject,
    backgroundColor: "rgba(0, 0, 0, 0.5)",
  },
});

export default _layout;
