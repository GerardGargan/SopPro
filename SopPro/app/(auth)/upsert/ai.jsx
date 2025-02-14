import { ScrollView, StyleSheet, Text, View } from "react-native";
import React, { useState } from "react";
import { ActivityIndicator, Button, TextInput } from "react-native-paper";
import { generateAiSop } from "../../../util/httpRequests";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useFocusEffect, useRouter } from "expo-router";
import InputErrorMessage from "./../../../components/UI/InputErrorMessage";
import { QueryClient } from "@tanstack/react-query";
import Toast from "react-native-toast-message";

const ai = () => {
  const [jobDescription, setJobDescription] = useState("");
  const [primaryGoal, setPrimaryGoal] = useState("");
  const [keyRisks, setKeyRisks] = useState("");

  const [jobDescriptionError, setJobDescriptionError] = useState(null);
  const [primaryGoalError, setPrimaryGoalError] = useState(null);
  const [keyRisksError, setKeyRisksError] = useState(null);

  const router = useRouter();
  const queryClient = useQueryClient();

  const isFocusedRef = React.useRef(true);

  useFocusEffect(
    React.useCallback(() => {
      isFocusedRef.current = true;
      return () => {
        isFocusedRef.current = false;
      };
    }, [])
  );

  const { mutate, isPending, isError } = useMutation({
    mutationFn: generateAiSop,
    onSuccess: (data) => {
      Toast.show({
        type: "success",
        text1: "AI Generated SOP is ready!",
        text2: "You can now view and edit the SOP",
        onPress: () => {
          router.replace({
            pathname: "/(auth)/upsert/[id]",
            params: {
              id: data.id,
            },
          });
        },
        visibilityTime: 5000,
      });
      queryClient.invalidateQueries(["sops"]);

      if (isFocusedRef.current) {
        router.replace({
          pathname: "/(auth)/upsert/[id]",
          params: {
            id: data.id,
          },
        });
      }
    },
    onError: () => {
      Toast.show({
        type: "error",
        text1: "Oops something went wrong!",
        text2: "The AI generator failed",
        visibilityTime: 5000,
      });
    },
  });

  function handleSubmit() {
    resetErrors();

    let isError = false;

    if (jobDescription.trim().length === 0) {
      setJobDescriptionError("Job description cant be left empty");
      isError = true;
    }
    if (primaryGoal.trim().length === 0) {
      setPrimaryGoalError("Primary goal cant be left empty");
      isError = true;
    }
    if (keyRisks.trim().length === 0) {
      setKeyRisksError("Key risks cant be left empty");
      isError = true;
    }

    if (isError) {
      return;
    }

    mutate({ jobDescription, primaryGoal, keyRisks });
  }

  function resetErrors() {
    setJobDescriptionError(null);
    setPrimaryGoalError(null);
    setKeyRisksError(null);
  }

  if (isPending) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator animating={true} />
      </View>
    );
  }

  return (
    <ScrollView style={styles.rootContainer}>
      <Text style={styles.title}>
        Provide Context for AI to Generate the SOP
      </Text>
      <Text style={styles.helperText}>
        Provide as much detail as possible about the job, including tasks,
        objectives, and risks involved. This will help the AI generate a more
        accurate and useful SOP.
      </Text>

      <TextInput
        style={[styles.textInput, styles.descInput]}
        label="Job Description"
        error={jobDescriptionError}
        placeholder="e.g. Operating a CNC machine to produce metal components"
        multiline
        numberOfLines={5}
        value={jobDescription}
        onChangeText={(value) => {
          setJobDescription(value);
          jobDescriptionError != null && setJobDescriptionError(null);
        }}
      />
      {jobDescriptionError && (
        <InputErrorMessage>{jobDescriptionError}</InputErrorMessage>
      )}

      <TextInput
        style={[styles.textInput]}
        error={primaryGoalError}
        label="Primary Objective or Goal"
        placeholder="e.g. Set up the machine efficiently and safely"
        multiline
        numberOfLines={3}
        value={primaryGoal}
        onChangeText={(value) => {
          setPrimaryGoal(value);
          primaryGoalError != null && setPrimaryGoalError(null);
        }}
      />
      {primaryGoalError && (
        <InputErrorMessage>{primaryGoalError}</InputErrorMessage>
      )}

      <TextInput
        style={[styles.textInput]}
        error={keyRisksError}
        label="Key Considerations or Risks"
        placeholder="e.g. Flying metal chips, wear safety glasses"
        multiline
        numberOfLines={3}
        value={keyRisks}
        onChangeText={(value) => {
          setKeyRisks(value);
          keyRisksError !== null && setKeyRisksError(null);
        }}
      />
      {keyRisksError && <InputErrorMessage>{keyRisksError}</InputErrorMessage>}

      <Button
        icon="lightbulb"
        mode="contained"
        contentStyle={{ height: 50 }}
        labelStyle={{ fontSize: 20 }}
        style={{ borderRadius: 0, marginVertical: 10 }}
        onPress={handleSubmit}
      >
        Generate SOP
      </Button>

      <Text style={styles.exampleText}>
        Example: Cleaning the factory floor involves sweeping and mopping the
        area, ensuring no hazardous spills remain. The goal is to maintain a
        safe work environment.
      </Text>
    </ScrollView>
  );
};

export default ai;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    padding: 20,
  },
  textInput: {
    marginTop: 16,
  },
  descInput: {
    height: 120,
  },
  title: {
    fontSize: 20,
    fontWeight: "bold",
    marginBottom: 8,
  },
  helperText: {
    fontSize: 14,
    color: "#6b6b6b",
    marginBottom: 12,
  },
  exampleText: {
    fontSize: 12,
    color: "#8a8a8a",
    marginTop: 20,
    fontStyle: "italic",
  },
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
});
