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
import { useSearchParams } from "expo-router/build/hooks";
import { TextInput, Button, IconButton } from "react-native-paper";

const Upsert = () => {
  const navigation = useNavigation();
  const { sopId } = useSearchParams();
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");

  useLayoutEffect(() => {
    navigation.setOptions({
      title: sopId == null ? "Create SOP" : "Edit SOP",
      headerRight: () => (
        <Button mode="contained" onPress={() => console.log("Saving..")}>
          {sopId == null ? "Create" : "Update"}
        </Button>
      ),
    });
  }, [navigation, sopId]);

  const handleSave = () => {
    // Handle save logic
    console.log("Title:", title);
    console.log("Description:", description);
  };

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
