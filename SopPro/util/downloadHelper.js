import * as FileSystem from "expo-file-system";
import * as Sharing from "expo-sharing";
import store from "../store";
import { Platform } from "react-native";

export async function downloadSopVersion(versionId, version, title) {
  const fileName = `${title}-V${version}-` + Date.now();

  const token = store.getState().auth.token;
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
