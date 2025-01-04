import React, { useState, useEffect } from "react";
import { StyleSheet, Text, View } from "react-native";
import { useMutation, useQuery } from "@tanstack/react-query";
import { Appbar, Button, Modal, Portal } from "react-native-paper";
import { useIsFocused } from "@react-navigation/native";
import Fab from "../../../components/sops/fab";
import SearchInput from "../../../components/UI/SearchInput.jsx";
import SopList from "../../../components/sops/SopList.jsx";
import { deleteSops } from "../../../util/httpRequests.js";

const APPBAR_HEIGHT = 50;

const Sops = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [debouncedSearchQuery, setDebouncedSearchQuery] = useState("");
  const [selectedIds, setSelectedIds] = useState([]);
  const [showDeleteWarning, setShowDeleteWarning] = useState(false);
  const statusFilter = 1;

  const isFocused = useIsFocused();

  const { data, isError, isPending, error } = useMutation({
    mutationFn: deleteSops,
    onSuccess: () => {
      console.log("Sops deleted successfully");
    },
  });

  useEffect(() => {
    const handler = setTimeout(() => setDebouncedSearchQuery(searchQuery), 500);
    return () => clearTimeout(handler);
  }, [searchQuery]);

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
    console.log("Deleting selected sops");
    setShowDeleteWarning(false);
    resetSelected();
  }

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
            <Button onPress={deleteSelected}>Yes</Button>
            <Button onPress={closeDeleteWarning}>No</Button>
          </View>
        </Modal>
      </Portal>

      <View style={styles.container}>
        <SearchInput value={searchQuery} onChangeText={setSearchQuery} />
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
        />
        {isFocused && <Fab />}
      </View>
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
});
