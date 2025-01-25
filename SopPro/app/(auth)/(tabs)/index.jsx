import { Text, View } from "react-native";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";

const index = () => {
  const isFocused = useIsFocused();

  return (
    <>
      <View style={{ justifyContent: "center", alignItems: "center" }}>
        <Text>Home</Text>
        <Text>Page content to be added...</Text>
      </View>
      {isFocused && <Fab />}
    </>
  );
};

export default index;
