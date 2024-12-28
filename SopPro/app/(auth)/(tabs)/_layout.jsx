import { View, Text } from "react-native";
import { Tabs } from "expo-router";
import React from "react";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import { FontAwesome5 } from "@expo/vector-icons";

function TabBarIcon({ ...props }) {
  return <FontAwesome5 size={28} style={{ marginBottom: -3 }} {...props} />;
}

const _layout = () => {
  return (
    <Tabs
      screenOptions={{
        headerShown: false,
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
  );
};

export default _layout;
