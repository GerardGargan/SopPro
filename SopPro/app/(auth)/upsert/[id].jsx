import { ScrollView, StyleSheet, Text, View } from "react-native";
import React, { useEffect, useLayoutEffect } from "react";
import {
  TextInput,
  Button,
  Modal,
  Portal,
  ActivityIndicator,
} from "react-native-paper";
import { useMutation, useQuery } from "@tanstack/react-query";
import { useLocalSearchParams, useNavigation, useRouter } from "expo-router";
import { createSop, fetchSop, updateSop } from "../../../util/httpRequests";
import Header from "../../../components/UI/Header";
import HazardItem from "../../../components/sops/upsert/hazardItem";

const Upsert = () => {
  const { id } = useLocalSearchParams();

  const [title, setTitle] = React.useState("");
  const [description, setDescription] = React.useState("");
  const [hazards, setHazards] = React.useState([]);
  const [selectedHazard, setSelectedHazard] = React.useState(null);

  const isCreate = id === "-1";

  const router = useRouter();
  const navigation = useNavigation();
  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create SOP" : "Edit SOP",
      headerRight: () => (
        <Button onPress={handleSave}>{isCreate ? "Save" : "Update"}</Button>
      ),
    });
  }, [navigation, handleSave]);

  const { data, isError, isLoading } = useQuery({
    enabled: !isCreate,
    queryKey: ["sop", id],
    queryFn: () => fetchSop(id),
  });

  const { mutate: mutateUpdate } = useMutation({
    mutationFn: updateSop,
    onSuccess: () => {
      router.navigate("/(auth)");
    },
  });

  const { mutate: mutateCreate } = useMutation({
    mutationFn: createSop,
    onSuccess: () => {
      router.navigate("/(auth)");
    },
  });

  useEffect(() => {
    if (data) {
      setTitle(data?.title || "");
      setDescription(data?.description || "");
      setHazards(
        data?.sopHazards.map((hazard) => {
          return { ...hazard, key: hazard.id };
        }) || []
      );
    }
  }, [data]);

  function handleTitleChange(text) {
    setTitle(text);
  }

  function handleDescriptionChange(text) {
    setDescription(text);
  }

  function handleSave() {
    const sop = {
      title: title,
      description: description,
      departmentId: 0,
      reference: "",
      isAiGenerated: false,
      sopHazards: hazards,
      id: +id,
    };

    if (isCreate) {
      mutateCreate(sop);
    } else {
      mutateUpdate(sop);
    }
  }

  function handleSelectHazard(id) {
    setSelectedHazard(id);
  }

  function handleAddHazard(hazard) {
    setHazards((prevState) => {
      const maxKey =
        prevState.length > 0
          ? Math.max(...prevState.map((hazard) => hazard.key))
          : 1;
      const newHazard = {
        id: null,
        key: maxKey + 1,
        name: "",
        controlMeasure: "",
        riskLevel: 1,
      };
      return [...prevState, newHazard];
    });
  }

  function handleUpdateHazard(key, identifier, value) {
    setHazards((prevState) => {
      const hazards = [...prevState];
      const index = hazards.findIndex((hazard) => hazard.key === key);
      hazards[index][identifier] = value;
      return hazards;
    });
  }

  function handleRemoveHazard(key) {
    setHazards((prevState) => {
      return prevState.filter((hazard) => hazard.key !== key);
    });
    setSelectedHazard(null);
  }

  if (isLoading) {
    return <ActivityIndicator animating={true} />;
  }

  if (isError) {
    return <Text>Error fetching SOP</Text>;
  }

  return (
    <ScrollView style={styles.rootContainer}>
      <TextInput
        style={styles.textInput}
        label="Title"
        placeholder="Enter title"
        value={title}
        onChangeText={(text) => handleTitleChange(text)}
      />
      <TextInput
        style={[styles.textInput, styles.descInput]}
        label="Description"
        placeholder="Enter description"
        multiline
        numberOfLines={10}
        value={description}
        onChangeText={(text) => handleDescriptionChange(text)}
        scrollEnabled={false}
      />
      <Header text="Safety information" textStyle={{ color: "black" }} />
      {hazards.map((hazard, index) => {
        return (
          <HazardItem
            key={hazard.key}
            hazard={hazard}
            onEdit={() => handleSelectHazard(hazard.key)}
          />
        );
      })}
      <Button icon="plus" onPress={handleAddHazard}>
        Add hazard
      </Button>
      <Portal>
        <Modal
          visible={selectedHazard !== null}
          onDismiss={() => setSelectedHazard(null)}
          contentContainerStyle={styles.modalContainer}
        >
          <Button
            icon="trash-can"
            onPress={() => handleRemoveHazard(selectedHazard)}
            style={{ alignSelf: "flex-end", marginBottom: 10 }}
            mode="contained"
          >
            Delete
          </Button>
          <TextInput
            label="Hazard"
            placeholder="Hazard description"
            style={styles.textInput}
            value={
              hazards.find((hazard) => hazard.key === selectedHazard)?.name
            }
            onChangeText={(text) =>
              handleUpdateHazard(selectedHazard, "name", text)
            }
          />
          <TextInput
            label="Control measure"
            placeholder="Control measure"
            style={[styles.textInput, styles.controlMeasureInput]}
            multiline
            numberOfLines={3}
            value={
              hazards.find((hazard) => hazard.key === selectedHazard)
                ?.controlMeasure
            }
            onChangeText={(text) =>
              handleUpdateHazard(selectedHazard, "controlMeasure", text)
            }
          />
          <Button mode="text" onPress={() => setSelectedHazard(null)}>
            Close
          </Button>
        </Modal>
      </Portal>
    </ScrollView>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  rootContainer: {
    margin: 20,
  },
  modalContainer: {
    backgroundColor: "white",
    padding: 20,
    margin: 20,
  },
  textInput: {
    marginBottom: 10,
  },
  descInput: {
    height: 150,
  },
  controlMeasureInput: {
    height: 100,
  },
});
