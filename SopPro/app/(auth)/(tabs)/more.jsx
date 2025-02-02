import { ScrollView, StyleSheet, Text, View } from "react-native";
import React from "react";
import { Link, useRouter } from "expo-router";
import MenuCard from "../../../components/UI/MenuCard";
import { KeyRound, LogOut } from "lucide-react-native";

const More = () => {
  const router = useRouter();

  const menuOptions = [
    {
      key: 1,
      icon: KeyRound,
      title: "Change password",
      onPress: () => {
        router.push("(auth)/user/changePassword");
      },
    },
    {
      key: 2,
      icon: LogOut,
      title: "Log out",
      onPress: () => {
        router.replace("/logout");
      },
    },
  ];

  return (
    <ScrollView style={styles.container}>
      {menuOptions.map((option) => {
        return (
          <MenuCard
            key={option.key}
            title={option.title}
            Icon={option.icon}
            onPress={option.onPress}
          />
        );
      })}
    </ScrollView>
  );
};

export default More;

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
});
