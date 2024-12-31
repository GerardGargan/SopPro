import { StyleSheet, Text, View } from "react-native";
import React from "react";
import StepCard from "./StepCard";
import { Button, FAB, IconButton, Portal } from "react-native-paper";
import { useState } from "react";

const EditSteps = ({ steps, setSteps }) => {
  const [selectedItem, setSelectedItem] = useState(null);

  function handlePositionMove(direction) {
    setSteps((prevState) => {
      const index = prevState.findIndex((step) => step.key === selectedItem);

      if (direction === "up" && index > 0) {
        const newSteps = [...prevState];
        const temp = newSteps[index];
        newSteps[index] = newSteps[index - 1];
        newSteps[index - 1] = temp;

        newSteps[index].position += 1;
        newSteps[index - 1].position -= 1;

        return newSteps;
      } else if (direction === "down" && index < prevState.length - 1) {
        const newSteps = [...prevState];
        const temp = newSteps[index];
        newSteps[index] = newSteps[index + 1];
        newSteps[index + 1] = temp;

        newSteps[index].position -= 1;
        newSteps[index + 1].position += 1;

        return newSteps;
      }

      return prevState;
    });
  }

  const ReorderSection = () => {
    return (
      <View style={{ flexDirection: "row", justifyContent: "space-between" }}>
        <View style={{ flexDirection: "row", justifyContent: "flex-start" }}>
          <IconButton
            mode="contained-tonal"
            icon="arrow-up-bold"
            size={30}
            onPress={() => {
              handlePositionMove("up");
            }}
          />
          <IconButton
            mode="contained-tonal"
            icon="arrow-down-bold"
            size={30}
            onPress={() => {
              handlePositionMove("down");
            }}
          />
        </View>
        <View>
          <Button
            mode="text"
            icon="check"
            style={{ fontSize: 30 }}
            contentStyle={{ height: 50 }}
            labelStyle={{ fontSize: 15 }}
            onPress={() => setSelectedItem(null)}
          >
            Done
          </Button>
        </View>
      </View>
    );
  };

  return (
    <>
      <View style={{ flex: 1 }}>
        {selectedItem && <ReorderSection />}
        {steps.map((step) => {
          return (
            <StepCard
              key={step.id}
              text={step.text}
              imageUrl={step.imageUrl}
              handleSelect={() => setSelectedItem(step.key)}
              isAnyItemSelected={selectedItem !== null}
              selected={selectedItem === step.key}
            />
          );
        })}
      </View>
      <Portal>
        <FAB
          icon="plus"
          style={styles.fab}
          onPress={() => console.log("Pressed")}
        />
      </Portal>
    </>
  );
};

export default EditSteps;

const styles = StyleSheet.create({
  fab: {
    position: "absolute",
    margin: 16,
    right: 0,
    bottom: 80,
  },
});
