import { StyleSheet, Text, View } from "react-native";
import React, { forwardRef, useCallback, useMemo, useState } from "react";
import {
  BottomSheetBackdrop,
  BottomSheetModal,
  BottomSheetView,
} from "@gorhom/bottom-sheet";
import MenuCard from "../../UI/MenuCard";
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
import {
  Star,
  ThumbsUp,
  ThumbsDown,
  Pencil,
  Trash2,
  FileCheck2,
  FileDown,
} from "lucide-react-native";
import ExportModal from "../exportModal/ExportModal";
import { useBottomSheetBackHandler } from "../../../hooks/useBottomSheetBackHandler";
import { ScrollView } from "react-native-gesture-handler";

const CustomBottomSheetModal = forwardRef((props, ref) => {
  const router = useRouter();
  const queryClient = useQueryClient();
  const snapPoints = useMemo(() => ["60%"], []);
  const theme = useTheme();
  const sop = props.sop;
  const sopVersions = sop?.sopVersions;
  const userRole = useSelector((state) => state.auth.role);
  const isAdmin = userRole === "admin";

  const { handleSheetPositionChange } = useBottomSheetBackHandler(ref);
  const [exportModalVisibile, setExportModalVisible] = useState(false);

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

  function handleExportPress() {
    closeSheet();
    setExportModalVisible(true);
  }

  const editCard = (
    <MenuCard Icon={Pencil} title="Edit" onPress={handleEditPress} />
  );

  const deleteCard = (
    <MenuCard Icon={Trash2} title="Delete" onPress={handleDeletePress} />
  );

  const favouritesCard = sop?.isFavourite ? (
    <MenuCard
      Icon={Star}
      title="Remove from favourites"
      onPress={handleRemoveFromFavouritesPress}
    />
  ) : (
    <MenuCard
      Icon={Star}
      title="Add to favourites"
      onPress={handleAddToFavouritesPress}
    />
  );

  const approvalCard = (
    <MenuCard Icon={ThumbsUp} title="Approve" onPress={handleApproval} />
  );

  const rejectApprovalCard = (
    <MenuCard Icon={ThumbsDown} title="Reject" onPress={handleRejectPress} />
  );

  const requestApprovalCard = (
    <MenuCard
      Icon={FileCheck2}
      title="Request Approval"
      onPress={handleRequestApproval}
    />
  );

  const exportCard = (
    <MenuCard Icon={FileDown} title="Export" onPress={handleExportPress} />
  );

  // Group cards based on user role
  const userCardStack = (
    <>
      {editCard}
      {(sop?.status === 1 || sop?.status === 5) && requestApprovalCard}
      {favouritesCard}
      {exportCard}
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
    <>
      <BottomSheetModal
        ref={ref}
        index={0}
        snapPoints={snapPoints}
        enablePanDownToClose={true}
        backdropComponent={renderBackdrop}
        onChange={handleSheetPositionChange}
      >
        <BottomSheetView style={styles.contentContainer}>
          <Text style={styles.headerText}>{sop?.title}</Text>
          <ScrollView
            style={{ flexGrow: 0 }}
            horizontal
            showsHorizontalScrollIndicator={false}
          >
            <CustomChip style={styles.customChip}>
              <Text>Version {sop?.version}</Text>
            </CustomChip>
            <CustomChip style={styles.customChip}>
              <Text>{getStatus(sop?.status)}</Text>
            </CustomChip>
            {sop?.isFavourite && (
              <CustomChip style={styles.customChip}>
                <View style={styles.rowContainer}>
                  <Star color="#888" size={17} />
                  <Text> Favourite</Text>
                </View>
              </CustomChip>
            )}
          </ScrollView>
          <Divider style={styles.divider} />
          <ScrollView style={{ width: "100%" }}>
            {isAdmin && adminCardStack}
            {userCardStack}
          </ScrollView>
        </BottomSheetView>
      </BottomSheetModal>

      {exportModalVisibile && (
        <ExportModal
          sopVersions={sopVersions}
          visible={exportModalVisibile}
          setVisibility={setExportModalVisible}
        />
      )}
    </>
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
    alignItems: "center",
  },
  customChip: {
    marginHorizontal: 3,
  },
});
