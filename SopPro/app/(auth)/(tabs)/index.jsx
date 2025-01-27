import { Text, View } from "react-native";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useSelector } from "react-redux";

const index = () => {
  const isFocused = useIsFocused();

  const userRole = useSelector((state) => state.auth.role);
  const userForename = useSelector((state) => state.auth.forename);

  return (
    <>
      <View style={{ justifyContent: "center", alignItems: "center" }}>
        <Text>Welcome {userForename}</Text>
        <Text>Page content to be added...</Text>
        <Text>You are an {userRole}</Text>
      </View>
      {isFocused && <Fab />}
    </>
  );
};

export default index;
