import React, { useState, useEffect, useCallback } from "react";
import { Platform, StyleSheet, Text, TextInput, View } from "react-native";
import { useQueryClient, useMutation, useQuery } from "@tanstack/react-query";
import { Appbar, Button, Modal, Portal } from "react-native-paper";
import { useIsFocused } from "@react-navigation/native";
import Fab from "../../../components/sops/fab";
import SopList from "../../../components/sops/SopList.jsx";
import { deleteSops } from "../../../util/httpRequests.js";
import Toast from "react-native-toast-message";
import CustomBottomSheetModal from "../../../components/sops/bottomSheet/CustomBottomSheetModal.jsx";
import { useRef } from "react";

const APPBAR_HEIGHT = 50;

const Sops = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [debouncedSearchQuery, setDebouncedSearchQuery] = useState("");
  const [selectedIds, setSelectedIds] = useState([]);
  const [showDeleteWarning, setShowDeleteWarning] = useState(false);
  const [bottomSheetSelectedSop, setBottomSheetSelectedSop] = useState(null);
  const statusFilter = 1;

  const isFocused = useIsFocused();
  const queryClient = useQueryClient();
  const bottomSheetModalRef = useRef();

  const { mutate: mutateDelete } = useMutation({
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

  useEffect(() => {
    const handler = setTimeout(() => setDebouncedSearchQuery(searchQuery), 500);
    return () => clearTimeout(handler);
  }, [searchQuery]);

  useEffect(() => {
    // reset selected ids when search query changes
    resetSelected();
  }, [debouncedSearchQuery]);

  function selectSop(id) {
    setSelectedIds((prevState) => {
      return [...prevState, id];
    });
  }

  function deselectSop(id) {
    setSelectedIds((prevState) => {
      return prevState.filter((sopId) => sopId !== id);
    });
  }

  function resetSelected() {
    setSelectedIds([]);
  }

  function onDeleteWarning() {
    setShowDeleteWarning(true);
  }

  function closeDeleteWarning() {
    setShowDeleteWarning(false);
  }

  function deleteSelected() {
    mutateDelete(selectedIds);
    setShowDeleteWarning(false);
    resetSelected();
  }

  const handlePresentModalPress = useCallback((sop) => {
    setBottomSheetSelectedSop(sop);
    bottomSheetModalRef.current?.present();
  }, []);

  return (
    <>
      <Portal>
        <Modal
          visible={showDeleteWarning}
          onDismiss={closeDeleteWarning}
          contentContainerStyle={styles.modalPaperContainer}
          dismissable={false}
          dismissableBackButton={false}
        >
          <Text style={styles.warningText}>
            Are you sure you want to delete the selected records?
          </Text>
          <Text style={styles.secondaryWarningText}>
            All versions will be deleted
          </Text>
          <View style={styles.deleteButtonsContainer}>
            <Button onPress={closeDeleteWarning}>No</Button>
            <Button onPress={deleteSelected}>Yes</Button>
          </View>
        </Modal>
      </Portal>

      <View style={styles.container}>
        <View style={styles.header}>
          <View style={styles.searchContainer}>
            <TextInput
              style={styles.searchInput}
              placeholder="Search"
              value={searchQuery}
              onChangeText={setSearchQuery}
              placeholderTextColor="#6b7280"
            />
          </View>
        </View>
        {selectedIds.length > 0 && (
          <Appbar style={{ height: APPBAR_HEIGHT }}>
            <Appbar.Content
              titleStyle={styles.selectedText}
              title={`${selectedIds.length} selected`}
            />
            <Appbar.Action icon="trash-can" onPress={onDeleteWarning} />
            <Appbar.Action icon="check" onPress={resetSelected} />
          </Appbar>
        )}

        <SopList
          debouncedSearchQuery={debouncedSearchQuery}
          statusFilter={statusFilter}
          searchQuery={searchQuery}
          selectedIds={selectedIds}
          selectSop={selectSop}
          deselectSop={deselectSop}
          openBottomSheet={handlePresentModalPress}
        />
        {isFocused && <Fab />}
      </View>

      <CustomBottomSheetModal
        ref={bottomSheetModalRef}
        sop={bottomSheetSelectedSop}
      />
    </>
  );
};

export default Sops;

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  selectedText: {
    fontSize: 16,
  },
  modalPaperContainer: {
    backgroundColor: "white",
    padding: 20,
    margin: 20,
  },
  deleteButtonsContainer: {
    flexDirection: "row",
    justifyContent: "space-evenly",
  },
  warningText: {
    textAlign: "center",
    fontSize: 18,
    marginBottom: 20,
  },
  secondaryWarningText: {
    textAlign: "center",
    fontSize: 13,
    marginBottom: 20,
    color: "red",
    fontWeight: "bold",
  },
  header: {
    flexDirection: "row",
    padding: 16,
    gap: 12,
    zIndex: 1,
  },
  searchContainer: {
    flex: 1,
  },
  searchInput: {
    backgroundColor: "#ffffff",
    borderRadius: 20,
    paddingHorizontal: 16,
    paddingVertical: 8,
    fontSize: 16,
    ...Platform.select({
      ios: {
        shadowColor: "#000",
        shadowOffset: { width: 0, height: 1 },
        shadowOpacity: 0.1,
        shadowRadius: 4,
      },
      android: {
        elevation: 2,
      },
    }),
  },
});
