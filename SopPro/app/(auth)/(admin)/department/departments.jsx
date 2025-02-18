import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import React, { useLayoutEffect } from "react";
import DepartmentList from "../../../../components/departments/DepartmentList";
import { useNavigation, useRouter } from "expo-router";
import { Button, useTheme } from "react-native-paper";
import { PlusCircle } from "lucide-react-native";

const departments = () => {
  const navigation = useNavigation();
  const router = useRouter();
  const theme = useTheme();

  useLayoutEffect(() => {
    navigation.setOptions({
      headerRight: () => (
        <TouchableOpacity style={styles.iconButton}>
          <PlusCircle
            size={30}
            color={theme.colors.primary}
            onPressIn={() => {
              router.navigate("(auth)/(admin)/department/-1");
            }}
          />
        </TouchableOpacity>
      ),
    });
  }, [navigation]);
  return (
    <View>
      <DepartmentList />
    </View>
  );
};

export default departments;

const styles = StyleSheet.create({
  iconButton: {
    marginRight: 10,
  },
});
