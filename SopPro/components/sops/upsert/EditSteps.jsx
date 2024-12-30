import { StyleSheet, Text, View } from "react-native";
import React from "react";
import StepCard from "./StepCard";

const EditSteps = ({ steps }) => {
  return (
    <View>
      {steps.map((step) => {
        return (
          <StepCard key={step.id} text={step.text} imageUrl={step.imageUrl} />
        );
      })}
      <StepCard text="Test" />
      <StepCard text="Test" />
      <StepCard text="Test" />
    </View>
  );
};

export default EditSteps;

const styles = StyleSheet.create({});
