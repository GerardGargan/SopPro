import { FlatList, ScrollView, StyleSheet, Text, View } from "react-native";
import React from "react";
import SopCardLargeSkeleton from "../skeletons/SopCardLargeSkeleton";
import SopCardLarge from "./SopCardLarge";

const SopHorizontalList = ({
  isFetched,
  data,
  isFetching,
  handlePresentModalPress,
  title,
  EmptyCard,
  callbackRoute,
  buttonText,
  emptyTitle,
  emptyText,
  EmptyIcon,
}) => {
  if (isFetched && data.length == 0) {
    return (
      <EmptyCard
        title={emptyTitle}
        buttonText={buttonText}
        callbackRoute={callbackRoute}
        text={emptyText}
        EmptyIcon={EmptyIcon}
      />
    );
  }

  return (
    <View style={styles.listContainer}>
      <Text style={styles.subtitleText}>{title}</Text>
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

      {isFetched && data.length > 0 && (
        <FlatList
          horizontal
          data={data}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <SopCardLarge
              sop={item}
              handleOpenBottomSheet={handlePresentModalPress}
            />
          )}
          contentContainerStyle={{
            paddingRight: 20,
          }}
          ListHeaderComponent={<View style={{ width: 8 }} />}
          showsHorizontalScrollIndicator={false}
        />
      )}
    </View>
  );
};

const styles = StyleSheet.create({
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

export default SopHorizontalList;
