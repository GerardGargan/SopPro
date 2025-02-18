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
import ConfirmationModal from "../../UI/ConfirmationModal";

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
  const [confirmationModal, setConfirmationModal] = useState({
    visible: false,
    title: "",
    subtitle: "",
    onCancel: () => {},
    onConfirm: () => {},
  });

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

  function openConfirmationModal(title, subtitle, callback) {
    setConfirmationModal({
      visible: true,
      title: title,
      subtitle: subtitle,
      onCancel: dismissConfirmationModal,
      onConfirm: callback,
    });
  }

  function dismissConfirmationModal() {
    setConfirmationModal({
      visible: false,
      title: "",
      subtitle: "",
      onCancel: () => {},
      onConfirm: () => {},
    });
  }

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
    openConfirmationModal(
      "Confirm Approval",
      "This SOP will be approved",
      () => {
        mutateApproveSop(sop.id);
        dismissConfirmationModal();
      }
    );
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
    openConfirmationModal(
      "Confirm SOP deletion",
      "All versions will be deleted permenantly",
      () => {
        deleteSopsMutation([sop.id]);
        dismissConfirmationModal();
      }
    );
  }

  function handleRequestApproval() {
    closeSheet();
    openConfirmationModal(
      "Confirm Approval Request",
      "Request approval for this SOP",
      () => {
        mutateRequestApproval(sop.id);
        dismissConfirmationModal();
      }
    );
  }

  function handleRejectPress() {
    closeSheet();
    openConfirmationModal(
      "Confirm rejection",
      "The author will be notified",
      () => {
        mutateReject(sop.id);
        dismissConfirmationModal();
      }
    );
  }

  function handleExportPress() {
    closeSheet();
    setExportModalVisible(true);
  }

  const userOptions = [
    {
      key: 1,
      title: "Edit",
      icon: Pencil,
      onPress: handleEditPress,
      visible: true,
    },
    {
      key: 2,
      title: "Request Approval",
      icon: FileCheck2,
      onPress: handleRequestApproval,
      visible: sop?.status === 1 || sop?.status === 5,
    },
    {
      key: 3,
      title: "Add to favourites",
      icon: Star,
      onPress: handleAddToFavouritesPress,
      visible: sop?.isFavourite === false,
    },
    {
      key: 4,
      title: "Remove from favourites",
      icon: Star,
      onPress: handleRemoveFromFavouritesPress,
      visible: sop?.isFavourite === true,
    },
    {
      key: 5,
      title: "Export",
      icon: FileDown,
      onPress: handleExportPress,
      visible: true,
    },
  ];

  const adminOptions = [
    {
      key: 1,
      title: "Approve",
      icon: ThumbsUp,
      onPress: handleApproval,
      visible: sop?.status === 2,
    },
    {
      key: 2,
      title: "Reject",
      icon: ThumbsDown,
      onPress: handleRejectPress,
      visible: sop?.status === 2,
    },
    {
      key: 3,
      title: "Delete",
      icon: Trash2,
      onPress: handleDeletePress,
      visible: true,
    },
  ];

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
            {userOptions.map((option) => {
              return (
                option.visible && (
                  <MenuCard
                    key={option.key}
                    title={option.title}
                    Icon={option.icon}
                    onPress={option.onPress}
                  />
                )
              );
            })}

            {isAdmin &&
              adminOptions.map((option) => {
                return (
                  option.visible && (
                    <MenuCard
                      key={option.key}
                      title={option.title}
                      Icon={option.icon}
                      onPress={option.onPress}
                    />
                  )
                );
              })}
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

      <ConfirmationModal
        visible={confirmationModal.visible}
        onConfirm={confirmationModal.onConfirm}
        onCancel={confirmationModal.onCancel}
        title={confirmationModal.title}
        subtitle={confirmationModal.subtitle}
      />
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
