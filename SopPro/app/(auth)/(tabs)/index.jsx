import { Text, View } from "react-native";
import React, { useCallback, useRef } from "react";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useRouter } from "expo-router";
import { Button } from "react-native-paper";
import CustomBottomSheetModal from "../../../components/sops/bottomSheet/CustomBottomSheetModal";

const index = () => {
  const isFocused = useIsFocused();
  const router = useRouter();
  const bottomSheetModalRef = useRef(null);

  const handlePresentModalPress = useCallback(() => {
    bottomSheetModalRef.current?.present();
  }, []);

  return (
    <>
      <View style={{ justifyContent: "center", alignItems: "center" }}>
        <Text>Home</Text>
        <Text>Page content to be added...</Text>
        <Button onPress={handlePresentModalPress}>Open bottom sheet</Button>
        <CustomBottomSheetModal ref={bottomSheetModalRef} />
      </View>
      {isFocused && <Fab />}
    </>
  );
};

export default index;
