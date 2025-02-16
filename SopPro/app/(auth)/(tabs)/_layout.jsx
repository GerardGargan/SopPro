import { Tabs } from "expo-router";
import React from "react";
import SafeAreaHeader from "../../../components/UI/SareAreaHeader";
import { useTheme } from "react-native-paper";
import { House, FileText, Grid2X2, ChartPie } from "lucide-react-native";

function TabBarIcon({ Icon, ...props }) {
  return <Icon size={28} style={{ marginBottom: -3 }} {...props} />;
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
            height: 60,
            paddingTop: 4,
          },
          tabBarActiveTintColor: theme.colors.primary,
          tabBarInactiveTintColor: theme.colors.onSurfaceVariant,
        }}
      >
        <Tabs.Screen
          name="index"
          options={{
            title: "Home",
            tabBarIcon: ({ color }) => (
              <TabBarIcon Icon={House} color={color} />
            ),
          }}
        />
        <Tabs.Screen
          name="analytics"
          options={{
            title: "Analytics",
            tabBarIcon: ({ color }) => (
              <TabBarIcon Icon={ChartPie} color={color} />
            ),
          }}
        />
        <Tabs.Screen
          name="sops"
          options={{
            title: "Sops",
            tabBarIcon: ({ color }) => (
              <TabBarIcon Icon={FileText} color={color} />
            ),
          }}
        />
        <Tabs.Screen
          name="more"
          options={{
            title: "More",
            tabBarIcon: ({ color }) => (
              <TabBarIcon Icon={Grid2X2} color={color} />
            ),
          }}
        />
      </Tabs>
    </>
  );
};

export default _layout;
