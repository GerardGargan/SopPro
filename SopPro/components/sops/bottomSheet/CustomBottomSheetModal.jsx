import { StyleSheet, Text, View } from "react-native";
import React, { forwardRef, useCallback, useMemo } from "react";
import {
  BottomSheetBackdrop,
  BottomSheetModal,
  BottomSheetView,
} from "@gorhom/bottom-sheet";
import BottomSheetCard from "./BottomSheetCard";
import { Divider, useTheme } from "react-native-paper";
import { useRouter } from "expo-router";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  addSopToFavourites,
  approveSop,
  deleteSops,
  rejectSop,
  removeSopFromFavourites,
  requestApproval,
} from "../../../util/httpRequests";
import Toast from "react-native-toast-message";
import { useSelector } from "react-redux";
import CustomChip from "../../UI/CustomChip";
import { getStatus } from "../../../util/statusHelper";
import { FontAwesome5 } from "@expo/vector-icons";

const CustomBottomSheetModal = forwardRef((props, ref) => {
  const router = useRouter();
  const queryClient = useQueryClient();
  const snapPoints = useMemo(() => ["60%"], []);
  const theme = useTheme();
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

  const { mutate: mutateApproveSop } = useMutation({
    mutationFn: approveSop,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Sop approved",
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

  const { mutate: mutateReject } = useMutation({
    mutationFn: rejectSop,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Sop rejected",
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

  const { mutate: mutateRequestApproval } = useMutation({
    mutationFn: requestApproval,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Review request sent",
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

  function handleApproval() {
    closeSheet();
    mutateApproveSop(sop.id);
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

  function handleRequestApproval() {
    closeSheet();
    mutateRequestApproval(sop.id);
  }

  function handleRejectPress() {
    closeSheet();
    mutateReject(sop.id);
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
      icon="thumbs-up"
      title="Approve"
      onPress={handleApproval}
    />
  );

  const rejectApprovalCard = (
    <BottomSheetCard
      icon="thumbs-down"
      title="Reject"
      onPress={handleRejectPress}
    />
  );

  const requestApprovalCard = (
    <BottomSheetCard
      icon="check-square"
      title="Request Approval"
      onPress={handleRequestApproval}
    />
  );

  // Group cards based on user role
  const userCardStack = (
    <>
      {editCard}
      {(sop?.status === 1 || sop?.status === 5) && requestApprovalCard}
      {favouritesCard}
      {deleteCard}
    </>
  );
  const adminCardStack = (
    <>
      {sop?.status === 2 && approvalCard}
      {sop?.status === 2 && rejectApprovalCard}
    </>
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
        <View style={styles.rowContainer}>
          <CustomChip style={styles.customChip}>
            <Text>Version {sop?.version}</Text>
          </CustomChip>
          <CustomChip style={styles.customChip}>
            <Text>{getStatus(sop?.status)}</Text>
          </CustomChip>
          {sop?.isFavourite && (
            <CustomChip style={styles.customChip}>
              <View style={styles.rowContainer}>
                <FontAwesome5 name="star" size={16} color="#888" />
                <Text> Favourite</Text>
              </View>
            </CustomChip>
          )}
        </View>
        <Divider style={styles.divider} />
        {isAdmin && adminCardStack}
        {userCardStack}
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
    marginBottom: 4,
  },
  divider: {
    width: "100%",
    marginVertical: 15,
  },
  rowContainer: {
    flexDirection: "row",
  },
  customChip: {
    marginHorizontal: 3,
  },
});
