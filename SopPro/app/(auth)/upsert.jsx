import {
  StyleSheet,
  Text,
  View,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
} from "react-native";
import React, { useLayoutEffect, useState } from "react";
import { useNavigation } from "expo-router";
import { useLocalSearchParams } from "expo-router";
import { TextInput, Button, ActivityIndicator } from "react-native-paper";
import { useQuery } from "@tanstack/react-query";
import { fetchSop } from "../../util/httpRequests";
import ErrorBlock from "../../components/UI/ErrorBlock";

const Upsert = () => {
  const navigation = useNavigation();
  const { sopId } = useLocalSearchParams();
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");

  useLayoutEffect(() => {
    navigation.setOptions({
      title: sopId == null ? "Create SOP" : "Edit SOP",
      headerRight: () => (
        <Button
          disabled={isError}
          mode="contained"
          onPress={() => console.log("Saving..")}
        >
          {sopId == null ? "Create" : "Update"}
        </Button>
      ),
    });
  }, [navigation, sopId, isError]);

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["sop", sopId],
    enabled: sopId != null,
    queryFn: () => fetchSop(sopId),
    onSuccess: (data) => {
      console.log(data);
    },
  });

  const handleSave = () => {
    // Handle save logic
    console.log("Title:", title);
    console.log("Description:", description);
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
      <ScrollView contentContainerStyle={styles.scrollView}>
        <View style={styles.form}>
          <TextInput
            style={{ marginBottom: 16 }}
            value={title}
            onChangeText={setTitle}
            label="Title"
            placeholder="Enter title"
          />
          <TextInput
            style={[styles.textArea, { marginBottom: 16 }]}
            value={description}
            onChangeText={setDescription}
            label="Description"
            placeholder="Enter description"
            multiline
            numberOfLines={10}
            textAlignVertical="top"
          />
          <Button title="Save" onPress={handleSave} />
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  scrollView: {
    flexGrow: 1,
    justifyContent: "center",
    padding: 16,
  },
  form: {
    flex: 1,
  },
  textArea: {
    height: 150,
  },
});
