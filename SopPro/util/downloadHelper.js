import * as FileSystem from "expo-file-system";
import * as Sharing from "expo-sharing";
import store from "../store";
import { Platform } from "react-native";

// Handles download an SOP Version
export async function downloadSopVersion(versionId, version, title) {
  // Give it a unique name including the date
  const fileName = `${title}-V${version}-` + Date.now();

  // Get the auth token to send in the authorization header
  const token = store.getState().auth.token;
  // Send the request
  try {
    const result = await FileSystem.downloadAsync(
      `${process.env.EXPO_PUBLIC_API_URL}/sop/${versionId}/pdf`,
      FileSystem.documentDirectory + fileName,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    if (result.status !== 200) {
      throw new Error(`Download failed with status ${result.status}`);
    }

    save(result.uri, fileName, result.headers["Content-Type"]);
  } catch (e) {
    const error = new Error("Download failed");
    throw error;
  }
}

// Saves the document to the devices file system
const save = async (uri, fileName, mimetype) => {
  if (Platform.OS === "android") {
    const permissions =
      await FileSystem.StorageAccessFramework.requestDirectoryPermissionsAsync();
    if (permissions.granted) {
      const base64 = await FileSystem.readAsStringAsync(uri, {
        encoding: FileSystem.EncodingType.Base64,
      });
      await FileSystem.StorageAccessFramework.createFileAsync(
        permissions.directoryUri,
        fileName,
        mimetype
      )
        .then(async (uri) => {
          await FileSystem.writeAsStringAsync(uri, base64, {
            encoding: FileSystem.EncodingType.Base64,
          });
        })
        .catch((e) => {
          console.log(e);
        });
    } else {
      Sharing.shareAsync(uri);
    }
  } else {
    Sharing.shareAsync(uri);
  }
};
