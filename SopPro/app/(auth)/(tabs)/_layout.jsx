import { Tabs } from "expo-router";
import React from "react";
import { FontAwesome5 } from "@expo/vector-icons";
import SafeAreaHeader from "../../../components/UI/SareAreaHeader";
import { Button, useTheme } from "react-native-paper";

function TabBarIcon({ ...props }) {
  return <FontAwesome5 size={28} style={{ marginBottom: -3 }} {...props} />;
}

const _layout = () => {
  const theme = useTheme();

  return (
    <>
      <SafeAreaHeader />
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
    </>
  );
};

export default _layout;
