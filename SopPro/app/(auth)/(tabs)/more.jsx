import React from "react";
import { ScrollView, StyleSheet, View } from "react-native";
import { useRouter } from "expo-router";
import { useSelector } from "react-redux";
import { Building, KeyRound, LogOut, UserPlus } from "lucide-react-native";
import MenuSection from "../../../components/UI/MenuSection";

const More = () => {
  const router = useRouter();
  const role = useSelector((state) => state.auth.role);

  const adminMenuOptions = [
    {
      key: 1,
      icon: UserPlus,
      title: "Invite user",
      onPress: () => router.push("(auth)/(admin)/invite"),
    },
    {
      key: 2,
      icon: Building,
      title: "Manage departments",
      onPress: () => router.push("(auth)/(admin)/department/departments"),
    },
  ];

  const userMenuOptions = [
    {
      key: 1,
      icon: KeyRound,
      title: "Change password",
      onPress: () => router.push("(auth)/user/changePassword"),
    },
    {
      key: 2,
      icon: LogOut,
      title: "Log out",
      onPress: () => router.replace("/logout"),
    },
  ];

  return (
    <ScrollView style={styles.container}>
      <View style={styles.content}>
        {role === "admin" && (
          <MenuSection
            title="Administrator Options"
            options={adminMenuOptions}
          />
        )}
        <MenuSection title="User Options" options={userMenuOptions} />
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  content: {
    padding: 16,
    gap: 24,
  },
  section: {
    gap: 12,
  },
});

export default More;
