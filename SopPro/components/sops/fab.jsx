import * as React from "react";
import { FAB, Portal } from "react-native-paper";
import { useRouter } from "expo-router";

const Fab = () => {
  const [state, setState] = React.useState({ open: false });

  const onStateChange = ({ open }) => setState({ open });
  var router = useRouter();

  const { open } = state;

  return (
    <Portal>
      <FAB.Group
        open={open}
        visible
        icon={open ? "file-document" : "plus"}
        style={{ position: "absolute", bottom: 60 }}
        actions={[
          {
            icon: "plus",
            label: "Start from scratch",
            onPress: () =>
              router.push({
                pathname: "/(auth)/upsert/[id]",
                params: {
                  id: -1,
                },
              }),
          },
          {
            icon: "lightbulb",
            label: "AI Generator",
            onPress: () =>
              router.push({
                pathname: "/(auth)/upsert/ai",
              }),
          },
        ]}
        onStateChange={onStateChange}
      />
    </Portal>
  );
};

export default Fab;
