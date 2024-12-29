import React, { useLayoutEffect, useState, useEffect } from "react";
import {
  StyleSheet,
  FlatList,
  View,
  KeyboardAvoidingView,
  Platform,
  Text,
  TouchableOpacity,
} from "react-native";
import { useNavigation, useRouter } from "expo-router";
import { useLocalSearchParams } from "expo-router";
import {
  TextInput,
  Button,
  ActivityIndicator,
  Modal,
  Portal,
} from "react-native-paper";
import { useQuery, useMutation } from "@tanstack/react-query";
import { fetchSop, updateSop } from "../../../util/httpRequests";
import ErrorBlock from "../../../components/UI/ErrorBlock";
import SOP from "../../../models/sop";
import Header from "../../../components/UI/Header";
import HazardItem from "../../../components/sops/upsert/hazardItem";

const Upsert = () => {
  const navigation = useNavigation();
  const { id } = useLocalSearchParams();
  const [sop, setSop] = useState(new SOP());
  const [selectedHazard, setSelectedHazard] = useState(null);

  const isCreate = id === "-1";

  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create SOP" : "Edit SOP",
      headerRight: () => (
        <Button disabled={isError} mode="contained" onPress={handleSave}>
          {isCreate ? "Create" : "Update"}
        </Button>
      ),
    });
  }, [navigation, id, isError]);

  // queries to api
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["sop", id],
    enabled: !isCreate,
    queryFn: () => fetchSop(id),
  });

  const {
    mutate,
    data: putData,
    isLoading: isPutLoading,
  } = useMutation({
    mutationFn: updateSop,
    onSuccess: () => {
      router.replace("/home");
    },
  });

  // set the state if data is loaded from the api
  useEffect(() => {
    if (data) {
      const loadedSop = new SOP(data);
      setSop(loadedSop);
    }
  }, [data]);

  const handleAddHazard = () => {
    setSop((prev) => {
      const maxId = prev.sopHazards.reduce(
        (max, hazard) => Math.max(max, hazard.id || 0),
        0
      );
      const newHazard = {
        id: maxId + 1,
        name: "New hazard",
        controlMeasure: "control measure",
      };
      return { ...prev, sopHazards: [...prev.sopHazards, newHazard] };
    });
  };

  const updateHazard = (id, key, value) => {
    setSop((prevState) => {
      const updatedHazards = prevState.sopHazards.map((hazard) => {
        if (hazard.id === id) {
          return { ...hazard, [key]: value };
        }
        return hazard;
      });
      return { ...prevState, sopHazards: updatedHazards };
    });
  };

  const handleSave = () => {
    console.log(sop);
    if (isCreate) {
      console.log("Create");
    } else {
      console.log("Update");
    }
  };

  if (isLoading) {
    return <ActivityIndicator animating={true} />;
  }

  if (isError) {
    return <ErrorBlock>{error.message}</ErrorBlock>;
  }

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === "ios" ? "padding" : "height"}
      style={styles.container}
    >
      <View>
        <TextInput
          style={styles.TextInput}
          value={sop.title}
          onChangeText={(text) =>
            setSop((prevState) => ({ ...prevState, title: text }))
          }
          label="Title"
          placeholder="Enter title"
        />
        <TextInput
          style={[styles.textArea, styles.TextInput]}
          value={sop.description}
          onChangeText={(text) =>
            setSop((prevState) => ({ ...prevState, description: text }))
          }
          label="Description"
          placeholder="Enter description"
          multiline
          numberOfLines={10}
          textAlignVertical="top"
        />
        <Header text="Safety information" textStyle={{ color: "black" }} />
      </View>
      <FlatList
        data={sop.sopHazards}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <HazardItem hazard={item} onEdit={setSelectedHazard} />
        )}
        ListEmptyComponent={
          <Text style={styles.emptyListText}>No hazards added yet.</Text>
        }
        ListFooterComponent={<View style={{ height: 80 }} />}
      />
      <TouchableOpacity style={styles.addButton} onPress={handleAddHazard}>
        <Text style={styles.addButtonText}>+</Text>
      </TouchableOpacity>
      <Portal>
        <Modal
          visible={selectedHazard !== null}
          onDismiss={() => setSelectedHazard(null)}
          contentContainerStyle={styles.modalContainerStyle}
        >
          <TextInput
            style={styles.TextInput}
            placeholder="Enter hazard"
            label="Hazard"
            value={
              selectedHazard &&
              sop.sopHazards.find((x) => x.id == selectedHazard)?.name
            }
            onChangeText={(text) => updateHazard(selectedHazard, "name", text)}
          />
          <TextInput
            label="Control measure"
            placeholder="Enter control measure"
            style={styles.TextInput}
            value={
              selectedHazard &&
              sop.sopHazards.find((x) => x.id == selectedHazard)?.controlMeasure
            }
            onChangeText={(text) =>
              updateHazard(selectedHazard, "controlMeasure", text)
            }
          />
          <Button
            mode="outlined"
            style={{ width: 100, alignSelf: "flex-end" }}
            onPress={() => setSelectedHazard(null)}
          >
            Save
          </Button>
        </Modal>
      </Portal>
    </KeyboardAvoidingView>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    margin: 16,
  },
  modalContainerStyle: {
    backgroundColor: "white",
    paddingVertical: 40,
    paddingHorizontal: 20,
    marginHorizontal: 20,
  },

  TextInput: {
    marginBottom: 16,
  },
  textArea: {
    height: 150,
  },
  emptyListText: {
    textAlign: "center",
    color: "gray",
    marginVertical: 16,
  },
  addButton: {
    position: "absolute",
    bottom: 16,
    right: 16,
    backgroundColor: "#6200ee",
    width: 56,
    height: 56,
    borderRadius: 28,
    alignItems: "center",
    justifyContent: "center",
    elevation: 4,
  },
  addButtonText: {
    color: "white",
    fontSize: 24,
    fontWeight: "bold",
  },
});
