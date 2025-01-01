import { useState } from "react";
import { Image, View, StyleSheet } from "react-native";
import * as ImagePicker from "expo-image-picker";
import { Icon, Button } from "react-native-paper";

export default function ImagePickerExample() {
  const [image, setImage] = useState(null);

  const pickImage = async () => {
    let result = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ["images"],
      allowsEditing: true,
      aspect: [4, 3],
      quality: 1,
    });

    if (!result.canceled) {
      setImage(result.assets[0].uri);
    }
  };

  const takePhoto = async () => {
    const { status } = await ImagePicker.requestCameraPermissionsAsync();
    if (status === "granted") {
      let result = await ImagePicker.launchCameraAsync({
        allowsEditing: true,
        aspect: [4, 3],
        quality: 1,
      });

      if (!result.canceled) {
        setImage(result.assets[0].uri);
      }
    }
  };

  console.log(image);

  return (
    <View style={styles.container}>
      {image ? (
        <Image source={{ uri: image }} style={styles.image} />
      ) : (
        <View style={[styles.image, styles.iconContainer]}>
          <Icon source="camera" size={45} />
        </View>
      )}
      <View style={styles.buttonContainer}>
        <Button mode="outlined" icon="file-image" onPress={pickImage}>
          From gallery
        </Button>
        <Button mode="outlined" icon="camera" onPress={takePhoto}>
          Take photo
        </Button>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: "center",
    justifyContent: "center",
  },
  buttonContainer: {
    flexDirection: "row",
    gap: 10,
    marginVertical: 20,
  },
  image: {
    width: 200,
    height: 200,
    marginTop: 20,
  },
  iconContainer: {
    backgroundColor: "lightgrey",
    justifyContent: "center",
    alignItems: "center",
  },
});
