import React, { useState, useEffect, useCallback } from "react";
import {
  Animated,
  Platform,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  TouchableWithoutFeedback,
  View,
} from "react-native";
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
  const [selectedStatus, setSelectedStatus] = useState({
    status: "all",
    val: null,
  });
  const [showStatusDropdown, setShowStatusDropdown] = useState(false);
  const statusFilter = 1;

  const isFocused = useIsFocused();
  const queryClient = useQueryClient();
  const bottomSheetModalRef = useRef();

  const dropdownAnimation = useRef(new Animated.Value(0)).current;
  const statuses = [
    { status: "all", val: null },
    { status: "Draft", val: 1 },
    { status: "Approved", val: 3 },
    { status: "In Review", val: 2 },
    { status: "Rejected", val: 4 },
  ];

  const toggleDropdown = () => {
    const toValue = showStatusDropdown ? 0 : 1;
    setShowStatusDropdown(!showStatusDropdown);
    Animated.spring(dropdownAnimation, {
      toValue,
      useNativeDriver: true,
      friction: 8,
      tension: 40,
    }).start();
  };

  const StatusDropdown = () => {
    return (
      <Animated.View
        style={[
          styles.dropdownContainer,
          {
            opacity: dropdownAnimation,
            transform: [
              {
                translateY: dropdownAnimation.interpolate({
                  inputRange: [0, 1],
                  outputRange: [-20, 0],
                }),
              },
            ],
          },
        ]}
      >
        {statuses.map((status) => (
          <TouchableOpacity
            key={status.val}
            style={styles.dropdownItem}
            onPress={() => {
              setSelectedStatus(status);
              toggleDropdown();
            }}
          >
            <Text
              style={[
                styles.dropdownText,
                status.status === selectedStatus.status &&
                  styles.dropdownTextSelected,
              ]}
            >
              {status.status === "all" ? "All Statuses" : status.status}
            </Text>
          </TouchableOpacity>
        ))}
      </Animated.View>
    );
  };

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
  }, [debouncedSearchQuery, selectedStatus]);

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

  const renderOverlay = () => {
    if (!showStatusDropdown) return null;

    return (
      <TouchableWithoutFeedback onPress={toggleDropdown}>
        <View style={styles.overlay} />
      </TouchableWithoutFeedback>
    );
  };

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
        {renderOverlay()}
        <View style={styles.header}>
          <View style={styles.searchContainer}>
            <TextInput
              style={styles.searchInput}
              placeholder="Search"
              value={searchQuery}
              onChangeText={setSearchQuery}
              placeholderTextColor="#6b7280"
            />
            {searchQuery.length > 0 && (
              <TouchableOpacity
                style={styles.clearButton}
                onPress={() => setSearchQuery("")}
              >
                <Text style={styles.clearButtonText}>✕</Text>
              </TouchableOpacity>
            )}
          </View>
          <View>
            <TouchableOpacity
              style={styles.filterButton}
              onPress={toggleDropdown}
            >
              <Text style={styles.filterButtonText}>
                {selectedStatus.status === "all"
                  ? "Filter"
                  : selectedStatus.status}
              </Text>
            </TouchableOpacity>
            {showStatusDropdown && <StatusDropdown />}
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
          statusFilter={selectedStatus.val}
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
    zIndex: 2,
  },
  overlay: {
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: "transparent",
    zIndex: 1,
  },
  searchContainer: {
    flex: 1,
    position: "relative",
  },
  searchInput: {
    backgroundColor: "#ffffff",
    borderRadius: 20,
    paddingHorizontal: 16,
    paddingVertical: 8,
    fontSize: 16,
    paddingRight: 40,
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
  clearButton: {
    position: "absolute",
    right: 12,
    top: "50%",
    transform: [{ translateY: -12 }], // Center vertically
    width: 24,
    height: 24,
    borderRadius: 12,
    backgroundColor: "#e5e7eb",
    alignItems: "center",
    justifyContent: "center",
  },
  clearButtonText: {
    fontSize: 14,
    color: "#6b7280",
    fontWeight: "600",
  },
  dropdownContainer: {
    position: "absolute",
    top: "100%",
    right: 0,
    backgroundColor: "#ffffff",
    borderRadius: 12,
    width: 180,
    marginTop: 4,
    ...Platform.select({
      ios: {
        shadowColor: "#000",
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 0.15,
        shadowRadius: 6,
      },
      android: {
        elevation: 4,
      },
    }),
  },
  dropdownItem: {
    paddingVertical: 12,
    paddingHorizontal: 16,
    borderBottomWidth: 1,
    borderBottomColor: "#f3f4f6",
  },
  dropdownText: {
    fontSize: 16,
    color: "#374151",
  },
  dropdownTextSelected: {
    color: "#2563eb",
    fontWeight: "500",
  },
  filterButton: {
    backgroundColor: "#ffffff",
    borderRadius: 20,
    paddingHorizontal: 16,
    paddingVertical: 8,
    justifyContent: "center",
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
  filterButtonText: {
    color: "#374151",
    fontSize: 16,
  },
});
