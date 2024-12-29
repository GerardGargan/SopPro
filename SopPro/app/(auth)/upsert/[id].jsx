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
import { TextInput, Button, ActivityIndicator } from "react-native-paper";
import { useQuery } from "@tanstack/react-query";
import { fetchSop } from "../../../util/httpRequests";
import ErrorBlock from "../../../components/UI/ErrorBlock";
import SOP from "../../../models/sop";
import Header from "../../../components/UI/Header";
import HazardItem from "../../../components/sops/hazardItem";

const Upsert = () => {
  const navigation = useNavigation();
  const { id } = useLocalSearchParams();
  const [sop, setSop] = useState(new SOP());

  const isCreate = !id;

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

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["sop", id],
    queryFn: () => fetchSop(id),
  });

  useEffect(() => {
    if (data) {
      const loadedSop = new SOP(data);
      setSop(loadedSop);
    }
  }, [data]);

  const handleSave = () => {
    console.log(isCreate ? "Create SOP" : "Update SOP");
  };

  const handleAddHazard = () => {
    const newHazard = {
      id: Date.now().toString(),
      name: "New hazard",
      controlMeasure: "control measure",
    };
    setSop((prev) => {
      return { ...prev, sopHazards: [...prev.sopHazards, newHazard] };
    });
  };

  const renderHeader = () => (
    <View>
      <TextInput
        style={styles.TextInput}
        value={sop.title}
        onChangeText={(text) => setSop({ ...sop, title: text })}
        label="Title"
        placeholder="Enter title"
      />
      <TextInput
        style={[styles.textArea, styles.TextInput]}
        value={sop.description}
        onChangeText={(text) => setSop({ ...sop, description: text })}
        label="Description"
        placeholder="Enter description"
        multiline
        numberOfLines={10}
        textAlignVertical="top"
      />
      <Header text="Safety information" textStyle={{ color: "black" }} />
    </View>
  );

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
      <FlatList
        data={sop.sopHazards}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <HazardItem hazard={item} onEdit={() => console.log("Edit", item)} />
        )}
        ListHeaderComponent={renderHeader}
        ListEmptyComponent={
          <Text style={styles.emptyListText}>No hazards added yet.</Text>
        }
        ListFooterComponent={<View style={{ height: 80 }} />}
      />
      <TouchableOpacity style={styles.addButton} onPress={handleAddHazard}>
        <Text style={styles.addButtonText}>+</Text>
      </TouchableOpacity>
    </KeyboardAvoidingView>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    margin: 16,
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
