import { FlatList, ScrollView, StyleSheet, Text, View } from "react-native";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useSelector } from "react-redux";
import Header from "../../../components/UI/Header";
import { capitiliseFirstLetter } from "../../../util/validationHelpers";
import { fetchSops } from "../../../util/httpRequests";
import { useQuery } from "@tanstack/react-query";
import SopCardLarge from "../../../components/sops/SopCardLarge";
import SopCardLargeSkeleton from "../../../components/skeletons/SopCardLargeSkeleton";

const index = () => {
  const isFocused = useIsFocused();
  const name = useSelector((state) => state.auth.forename);

  const { data, isFetching, isFetched, isError, error } = useQuery({
    queryKey: ["sops", "favourites"],
    queryFn: () => fetchSops({ isFavourite: true }),
  });

  return (
    <ScrollView style={styles.rootContainer}>
      <Header
        textStyle={{ color: "black" }}
        text={"Welcome " + capitiliseFirstLetter(name)}
      />
      <Text style={styles.subtitleText}>Favourites</Text>
      {isFetching && (
        <ScrollView
          showsHorizontalScrollIndicator={false}
          horizontal
          style={styles.skeletonContainer}
          contentContainerStyle={{
            paddingRight: 20,
          }}
        >
          <View style={{ width: 8 }} />
          {[...Array(4)].map((_, index) => (
            <SopCardLargeSkeleton key={index} />
          ))}
        </ScrollView>
      )}

      {isFetched && (
        <FlatList
          horizontal
          data={data}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => <SopCardLarge sop={item} />}
          contentContainerStyle={{
            paddingRight: 20,
          }}
          ListHeaderComponent={<View style={{ width: 8 }} />}
          showsHorizontalScrollIndicator={false}
        />
      )}
      {isFocused && <Fab />}
    </ScrollView>
  );
};

export default index;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
  },
  listContainer: {
    paddingVertical: 16,
  },
  skeletonContainer: {
    paddingVertical: 16,
  },
  subtitleText: {
    fontSize: 22,
    fontWeight: "700",
    marginLeft: 16,
    marginBottom: 8,
    color: "#333",
    letterSpacing: 0.5,
  },
});
