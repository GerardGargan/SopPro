import { StyleSheet, Text, View } from "react-native";
import React, { forwardRef, useCallback, useMemo } from "react";
import {
  BottomSheetBackdrop,
  BottomSheetModal,
  BottomSheetView,
} from "@gorhom/bottom-sheet";
import BottomSheetCard from "./BottomSheetCard";
import { Divider } from "react-native-paper";
import { useRouter } from "expo-router";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { addSopToFavourites } from "../../../util/httpRequests";
import Toast from "react-native-toast-message";

const CustomBottomSheetModal = forwardRef((props, ref) => {
  const router = useRouter();
  const queryClient = useQueryClient();
  const snapPoints = useMemo(() => ["60%"], []);
  const sop = props.sop;

  function closeSheet() {
    ref.current?.close();
  }

  const renderBackdrop = useCallback(
    (props) => (
      <BottomSheetBackdrop
        {...props}
        onPress={() => ref.current?.close()}
        disappearsOnIndex={-1}
      />
    ),
    []
  );

  const { mutate: addFavouriteMutation } = useMutation({
    mutationFn: addSopToFavourites,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Added to favourites",
        visibilityTime: 3000,
      });
      queryClient.invalidateQueries("sops");
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  // TODO -> Later organise first by creating each card and storing in variables
  // then group into admin and normal user cards/stacks
  // then render based on the user role

  function handleEditPress() {
    closeSheet();
    router.push({
      pathname: "/(auth)/upsert/[id]",
      params: {
        id: sop.id,
      },
    });
  }

  function handleAddToFavouritesPress() {
    closeSheet();
    addFavouriteMutation(sop.id);
  }

  function handleRemoveFromFavouritesPress() {
    closeSheet();
    console.log("Remove from favourites");
  }

  const favouritesCard = sop?.isFavourite ? (
    <BottomSheetCard
      icon="star"
      solid
      title="Remove from favourites"
      onPress={handleRemoveFromFavouritesPress}
    />
  ) : (
    <BottomSheetCard
      icon="star"
      solid
      title="Add to favourites"
      onPress={handleAddToFavouritesPress}
    />
  );

  return (
    <BottomSheetModal
      ref={ref}
      index={0}
      snapPoints={snapPoints}
      enablePanDownToClose={true}
      backdropComponent={renderBackdrop}
    >
      <BottomSheetView style={styles.contentContainer}>
        <Text style={styles.headerText}>{sop?.title}</Text>
        <Divider style={styles.divider} />
        <BottomSheetCard icon="edit" title="Edit" onPress={handleEditPress} />
        {favouritesCard}
      </BottomSheetView>
    </BottomSheetModal>
  );
});

export default CustomBottomSheetModal;

const styles = StyleSheet.create({
  contentContainer: {
    flex: 1,
    alignItems: "center",
  },
  containerHeadline: {
    fontSize: 24,
    fontWeight: 600,
    padding: 20,
  },
  headerText: {
    fontSize: 24,
    fontWeight: "bold",
  },
  divider: {
    width: "100%",
    marginVertical: 20,
  },
});
