import { StyleSheet, Text, View, FlatList } from "react-native";
import React from "react";
import { useInfiniteQuery } from "@tanstack/react-query";
import { fetchSops } from "../../util/httpRequests";
import SopCard from "./SopCard";
import { ActivityIndicator } from "react-native-paper";
import ErrorBlock from "../UI/ErrorBlock";

const PAGE_SIZE = 20;

const SopList = ({
  debouncedSearchQuery,
  statusFilter,
  searchQuery,
  selectedIds,
  selectSop,
  deselectSop,
  openBottomSheet,
}) => {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isPending,
    isError,
    error,
  } = useInfiniteQuery({
    queryKey: ["sops", { debouncedSearchQuery, statusFilter }],
    queryFn: ({ pageParam = 1 }) =>
      fetchSops({
        searchQuery: debouncedSearchQuery,
        statusFilter,
        page: pageParam,
        pageSize: PAGE_SIZE,
      }),
    getNextPageParam: (lastPage, allPages) => {
      if (lastPage.length < PAGE_SIZE) {
        return undefined;
      }
      return allPages.length + 1;
    },
    enabled: !!debouncedSearchQuery || searchQuery === "",
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
  });

  function toggleSelect(id) {
    if (selectedIds.includes(id)) {
      deselectSop(id);
    } else {
      selectSop(id);
    }
  }

  // Flatten the pages array into a single array of items
  const flattenedData = data?.pages.flatMap((page) => page) ?? [];

  const loadMore = () => {
    if (hasNextPage && !isFetchingNextPage) {
      fetchNextPage();
    }
  };

  const renderFooter = () => {
    if (!isFetchingNextPage) return null;
    return (
      <View style={styles.footer}>
        <ActivityIndicator animating={true} />
      </View>
    );
  };

  if (isPending) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  if (isError) {
    return (
      <View style={styles.centered}>
        <ErrorBlock>{error.message}</ErrorBlock>
      </View>
    );
  }

  return (
    <>
      {flattenedData.length === 0 && (
        <Text style={styles.noSopText}>No items found</Text>
      )}
      <FlatList
        data={flattenedData}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <SopCard
            sop={item}
            toggleSelect={toggleSelect}
            selected={selectedIds.includes(item.id)}
            isSelectedItems={selectedIds.length > 0}
            openBottomSheet={openBottomSheet}
          />
        )}
        onEndReached={loadMore}
        onEndReachedThreshold={0.3}
        ListFooterComponent={renderFooter}
      />
    </>
  );
};

export default SopList;

const styles = StyleSheet.create({
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  noSopText: {
    marginTop: 10,
    textAlign: "center",
    fontSize: 20,
  },
  footer: {
    padding: 10,
    justifyContent: "center",
    alignItems: "center",
  },
});
