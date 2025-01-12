import * as React from "react";
import { StyleSheet, View, Pressable } from "react-native";
import { Appbar, Text, useTheme } from "react-native-paper";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const BOTTOM_APPBAR_HEIGHT = 60;

const BottomBar = ({ selectedScreen, onSelectScreen }) => {
  const { bottom } = useSafeAreaInsets();
  const theme = useTheme();

  return (
    <Appbar
      style={[
        styles.bottom,
        {
          height: BOTTOM_APPBAR_HEIGHT + bottom,
          backgroundColor: theme.colors.surface,
        },
      ]}
      safeAreaInsets={{ bottom }}
    >
      <View style={styles.rootContainer}>
        <Pressable
          style={[
            styles.buttonContainer,
            selectedScreen === "overview" && {
              backgroundColor: theme.colors.surfaceVariant,
            },
          ]}
          onPress={() => onSelectScreen("overview")}
        >
          <Text style={styles.buttonText}>Edit overview</Text>
        </Pressable>

        <Pressable
          style={[
            styles.buttonContainer,
            selectedScreen === "steps" && {
              backgroundColor: theme.colors.surfaceVariant,
            },
          ]}
          onPress={() => onSelectScreen("steps")}
        >
          <Text style={styles.buttonText}>Edit steps</Text>
        </Pressable>
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
  rootContainer: {
    width: "100%",
    flexDirection: "row",
    height: "100%",
  },
  buttonContainer: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  buttonText: {
    fontSize: 16,
  },
});

export default BottomBar;
