import * as React from "react";
import { StyleSheet, View } from "react-native";
import { Appbar, Button, FAB, useTheme } from "react-native-paper";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const BOTTOM_APPBAR_HEIGHT = 80;
const MEDIUM_FAB_HEIGHT = 56;

const BottomBar = ({ selectedScreen, onSelectScreen }) => {
  const { bottom } = useSafeAreaInsets();
  const theme = useTheme();

  return (
    <Appbar
      style={[
        styles.bottom,
        {
          height: BOTTOM_APPBAR_HEIGHT + bottom,
          backgroundColor: theme.colors.elevation.level2,
        },
      ]}
      safeAreaInsets={{ bottom }}
    >
      <View style={styles.buttonContainer}>
        <View>
          {selectedScreen === "steps" && (
            <Button
              icon="arrow-left"
              mode="text"
              onPress={() => onSelectScreen("overview")}
            >
              Edit overview
            </Button>
          )}
        </View>

        <View>
          {selectedScreen === "overview" && (
            <Button
              icon="arrow-right"
              mode="text"
              contentStyle={{ flexDirection: "row-reverse" }}
              onPress={() => onSelectScreen("steps")}
            >
              Edit steps
            </Button>
          )}
        </View>
      </View>
    </Appbar>
  );
};

const styles = StyleSheet.create({
  bottom: {
    backgroundColor: "aquamarine",
    position: "absolute",
    left: 0,
    right: 0,
    bottom: 0,
  },
  fab: {
    position: "absolute",
    right: 16,
  },
  buttonContainer: {
    width: "100%",
    flexDirection: "row",
    justifyContent: "space-between",
  },
});

export default BottomBar;
