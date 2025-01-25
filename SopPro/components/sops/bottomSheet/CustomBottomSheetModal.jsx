import { StyleSheet, Text } from "react-native";
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
import {
  addSopToFavourites,
  deleteSops,
  removeSopFromFavourites,
} from "../../../util/httpRequests";
import Toast from "react-native-toast-message";
import { useSelector } from "react-redux";

const CustomBottomSheetModal = forwardRef((props, ref) => {
  const router = useRouter();
  const queryClient = useQueryClient();
  const snapPoints = useMemo(() => ["60%"], []);
  const sop = props.sop;
  const userRole = useSelector((state) => state.auth.role);
  const isAdmin = userRole === "admin";

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

  const { mutate: removeFavouriteMutation } = useMutation({
    mutationFn: removeSopFromFavourites,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Removed from favourites",
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

  const { mutate: deleteSopsMutation } = useMutation({
    mutationFn: deleteSops,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Deleted successfully",
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
    removeFavouriteMutation(sop.id);
  }

  function handleDeletePress() {
    closeSheet();
    deleteSopsMutation([sop.id]);
  }

  function handleApproval() {
    closeSheet();
    console.log("Approval goes here..");
  }

  const editCard = (
    <BottomSheetCard icon="edit" title="Edit" onPress={handleEditPress} />
  );

  const deleteCard = (
    <BottomSheetCard
      icon="trash-alt"
      title="Delete"
      onPress={handleDeletePress}
    />
  );

  const favouritesCard = sop?.isFavourite ? (
    <BottomSheetCard
      icon="star"
      title="Remove from favourites"
      onPress={handleRemoveFromFavouritesPress}
    />
  ) : (
    <BottomSheetCard
      icon="star"
      title="Add to favourites"
      onPress={handleAddToFavouritesPress}
    />
  );

  const approvalCard = (
    <BottomSheetCard
      icon="check-square"
      title="Approve"
      onPress={handleApproval}
    />
  );

  // Group cards based on user role
  const userCardStack = (
    <>
      {editCard}
      {favouritesCard}
      {deleteCard}
    </>
  );
  const adminCardStack = <>{approvalCard}</>;

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
        {userCardStack}
        {isAdmin && adminCardStack}
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
