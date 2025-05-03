import { StyleSheet, Text, View } from "react-native";
import React, { useEffect, useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import {
  createSetting,
  getSettingByKey,
  updateSetting,
  uploadImage,
} from "../../../../util/httpRequests";
import { ActivityIndicator } from "react-native-paper";
import ErrorBlock from "../../../../components/UI/ErrorBlock";
import ImagePickerComponent from "../../../../components/UI/ImagePicker";
import Toast from "react-native-toast-message";

const index = () => {
  const [imageUrl, setImageUrl] = useState(null);
  const isCreate = imageUrl === null;

  // Fetch existing logo
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["setting", "logo"],
    queryFn: () => getSettingByKey("logo"),
  });

  // Set state when data is fetched
  useEffect(() => {
    if (data) {
      setImageUrl(data.result.value);
    } else {
      setImageUrl(null);
    }
  }, [data]);

  // Mutation hook for uploading image/logo and triggering mutation to add/update the setting
  const { mutate: mutateUpload } = useMutation({
    mutationFn: uploadImage,
    onSuccess: (data) => {
      setImageUrl(data.result);
      mutateSetting({ type: "tenancy", key: "logo", value: data.result });
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  // Hook for updating/creating the setting
  const { mutate: mutateSetting } = useMutation({
    mutationFn: isCreate ? createSetting : updateSetting,
    onSuccess: () => {
      Toast.show({
        type: "success",
        text1: "Success",
        text2: "Setting saved",
        visibilityTime: 3000,
      });
    },
    onError: () => {
      Toast.show({
        type: "error",
        text1: "Error",
        text2: "Error saving setting",
        visibilityTime: 3000,
      });
    },
  });

  // Function to upload the image
  function handleImageUpload(imageUri) {
    const formData = new FormData();
    formData.append("file", {
      uri: imageUri,
      name: "image.jpg",
      type: "image/jpeg",
    });

    mutateUpload(formData);
  }

  // Show loading spinner if fetching data
  if (isPending) {
    return <ActivityIndicator animating={true} />;
  }

  // Show error message if an error exists
  if (isError) {
    return <ErrorBlock>{error.message}</ErrorBlock>;
  }

  // Render the screen with the image picker
  return (
    <View style={styles.rootContainer}>
      <Text style={styles.title}>Upload a custom logo</Text>
      <ImagePickerComponent
        imageUrl={imageUrl}
        onSelect={(image) => handleImageUpload(image)}
        allowsEditing={false}
      />
    </View>
  );
};

export default index;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    margin: 30,
    paddingTop: 50,
  },
  title: {
    textAlign: "center",
    fontSize: 20,
    marginBottom: 6,
  },
});
