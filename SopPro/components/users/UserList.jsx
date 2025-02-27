import { FlatList, StyleSheet, Text, View } from "react-native";
import React from "react";
import { useQuery } from "@tanstack/react-query";
import { fetchAllUsers } from "../../util/httpRequests";
import { ActivityIndicator } from "react-native-paper";
import ErrorBlock from "../UI/ErrorBlock";
import EntityCard from "../UI/EntityCard";
import { User } from "lucide-react-native";
import { useRouter } from "expo-router";

const UserList = () => {
  const router = useRouter();

  const { data, isFetching, isError, error } = useQuery({
    queryKey: ["users"],
    queryFn: fetchAllUsers,
  });

  if (isFetching) {
    return (
      <View style={styles.loader}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  if (isError) {
    return (
      <View style={styles.errorContainer}>
        <ErrorBlock>
          <Text>{error?.message}</Text>
        </ErrorBlock>
      </View>
    );
  }

  return (
    <FlatList
      data={data}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => (
        <EntityCard
          name={`${item.forename} ${item.surname}`}
          Icon={User}
          onPress={() => router.navigate(`(auth)/(admin)/user/${item.id}`)}
        />
      )}
    />
  );
};

export default UserList;

const styles = StyleSheet.create({
  loader: {
    flex: 1,
    justifyContent: "center",
  },
  errorContainer: {
    marginHorizontal: 20,
  },
});
