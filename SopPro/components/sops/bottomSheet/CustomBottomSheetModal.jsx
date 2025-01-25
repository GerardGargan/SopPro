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

const CustomBottomSheetModal = forwardRef((props, ref) => {
  const router = useRouter();
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
    console.log("Add to favourites");
  }

  function handleRemoveFromFavouritesPress() {
    closeSheet();
    console.log("Remove from favourites");
  }

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
        <BottomSheetCard
          icon="star"
          title="Add to favourites"
          onPress={handleAddToFavouritesPress}
        />
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
