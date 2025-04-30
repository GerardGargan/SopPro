import { ScrollView, StyleSheet, Text, View } from "react-native";
import React, { useState, useEffect, useLayoutEffect } from "react";
import { Button, ActivityIndicator } from "react-native-paper";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useLocalSearchParams, useNavigation, useRouter } from "expo-router";
import {
  createSop,
  fetchDepartments,
  fetchPpe,
  fetchSop,
  updateSop,
} from "../../../util/httpRequests";
import EditOverview from "../../../components/sops/upsert/overview/EditOverview";
import BottomBar from "../../../components/sops/upsert/BottomBar";
import EditSteps from "../../../components/sops/upsert/steps/EditSteps";
import ErrorBlock from "../../../components/UI/ErrorBlock";
import Toast from "react-native-toast-message";

const Upsert = () => {
  const { id } = useLocalSearchParams();

  // State to manage
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [hazards, setHazards] = useState([]);
  const [steps, setSteps] = useState([]);
  const [selectedHazard, setSelectedHazard] = useState(null);
  const [screen, setScreen] = useState("overview");
  const [version, setVersion] = useState(1);
  const [status, setStatus] = useState(1);
  const [departments, setDepartments] = useState([]);
  const [selectedDepartment, setSelectedDepartment] = useState(null);
  const [ppeList, setPpeList] = useState([]);

  // If the id is -1 we are creating an SOP, otherwise we are editing an existing SOP
  const isCreate = id === "-1";

  // Hooks
  const router = useRouter();
  const navigation = useNavigation();
  const queryClient = useQueryClient();

  // Set screen title based on isCreate or Update
  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create SOP" : "Edit SOP",
      headerRight: () => (
        <Button disabled={isError && isFetched} onPressIn={handleSave}>
          {isCreate ? "Save" : "Update"}
        </Button>
      ),
    });
  }, [navigation, handleSave, isError, isFetched]);

  // Fetch existing sop if we are editing an existing SOP
  const { data, isError, isFetching, isFetched, error } = useQuery({
    enabled: !isCreate,
    queryKey: ["sop", id],
    queryFn: () => fetchSop(id),
  });

  // Fetch departments for dropdown
  const { data: departmentsData } = useQuery({
    queryKey: ["departments"],
    queryFn: fetchDepartments,
  });

  // Fetch PPE for dropdown
  const { data: ppeData } = useQuery({
    queryKey: ["ppe"],
    queryFn: fetchPpe,
  });

  // Mutation for updating an SOP
  const {
    mutate: mutateUpdate,
    isPending: isPendingPut,
    isError: isErrorPut,
    error: errorPut,
  } = useMutation({
    mutationFn: updateSop,
    onSuccess: () => {
      queryClient.invalidateQueries(["sops"]);
      Toast.show({
        type: "success",
        text1: "SOP updated successfully",
        visibilityTime: 3000,
      });
      router.replace("/(auth)");
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  // Mutation for creating an SOP
  const {
    mutate: mutateCreate,
    isPending: isPendingPost,
    isError: isErrorPost,
    error: errorPost,
  } = useMutation({
    mutationFn: createSop,
    onSuccess: () => {
      queryClient.invalidateQueries(["sops"]);
      Toast.show({
        type: "success",
        text1: "SOP created successfully",
        visibilityTime: 3000,
      });
      router.replace("/(auth)");
    },
    onError: (error) => {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    },
  });

  // Once data is loaded populate the state we will work with
  useEffect(() => {
    if (data) {
      setTitle(data?.title || "");
      setDescription(data?.description || "");
      setSelectedDepartment(data?.departmentId || null);
      setHazards(
        data?.sopHazards.map((hazard) => {
          return { ...hazard, key: hazard.id };
        }) || []
      );
      setStatus(data?.status || 1);
      setVersion(data?.version || 1);
      setSteps(
        // sort steps by position
        data?.sopSteps
          .sort((a, b) => a.position - b.position)
          .map((step) => {
            return { ...step, key: step.id };
          }) || []
      );
    }
  }, [data]);

  useEffect(() => {
    if (departmentsData) {
      setDepartments(departmentsData);
    }
  }, [departmentsData]);

  useEffect(() => {
    if (ppeData) {
      setPpeList(ppeData);
    }
  }, [ppeData]);

  // Update title state when the title is changed
  function handleTitleChange(text) {
    setTitle(text);
  }

  // Update description state when the title is changed
  function handleDescriptionChange(text) {
    setDescription(text);
  }

  // Save the SOP after performing validation
  function handleSave() {
    const errors = [];
    console.log("title", title);

    if (!title || title.trim() === "") {
      errors.push("Title is required");
    }

    if (!description || description.trim() === "") {
      errors.push("Description is required");
    }

    if (errors.length > 0) {
      Toast.show({
        type: "error",
        text1: errors.join("\n"),
        visibilityTime: 3000,
      });
      return;
    }

    // Set up SOP data model expected by the API
    const sop = {
      title: title,
      description: description,
      departmentId: 0,
      reference: Math.random().toString(36).substring(7),
      isAiGenerated: false,
      sopHazards: hazards,
      sopSteps: steps,
      departmentId: selectedDepartment,
      id: +id,
    };

    if (isCreate) {
      mutateCreate(sop);
    } else {
      mutateUpdate(sop);
    }
  }

  // Handle selecting a hazard
  function handleSelectHazard(id) {
    setSelectedHazard(id);
  }

  // Function for adding a new hazard to the state
  function handleAddHazard(hazard) {
    setHazards((prevState) => {
      const maxKey =
        prevState.length > 0
          ? Math.max(...prevState.map((hazard) => hazard.key))
          : 1;
      const newHazard = {
        id: null,
        key: maxKey + 1,
        name: "",
        controlMeasure: "",
        riskLevel: 1,
      };
      return [...prevState, newHazard];
    });
  }

  // Handle updating an existing hazard
  function handleUpdateHazard(key, identifier, value) {
    setHazards((prevState) => {
      const hazards = [...prevState];
      const index = hazards.findIndex((hazard) => hazard.key === key);
      hazards[index][identifier] = value;
      return hazards;
    });
  }

  // Handle deleting a hazard
  function handleRemoveHazard(key) {
    setHazards((prevState) => {
      return prevState.filter((hazard) => hazard.key !== key);
    });
    setSelectedHazard(null);
  }

  // Handle selecting a department and updating the state
  function handleSelectDepartment(departmentId) {
    if (departmentId === -1) {
      setSelectedDepartment(null);
    } else {
      setSelectedDepartment(departmentId);
    }
  }

  // Handle selecting which screen the user is on (Overview or Tabs)
  function selectScreen(screen) {
    setScreen(screen);
  }

  function getUpdateErrorMessage() {
    if (isErrorPut) return errorPut.message;
    if (isErrorPost) return errorPost.message;
    return null;
  }

  // Handle editing a steps PPE
  function handleEditStepPpe(stepKey, ppe) {
    setSteps((prevState) => {
      const index = prevState.findIndex((step) => step.key === stepKey);
      const newSteps = [...prevState];
      newSteps[index].ppeIds = ppe || [];
      return newSteps;
    });
  }

  const errorMessage = getUpdateErrorMessage();

  // Show loading spinner if query is pending
  if (isFetching || isPendingPut || isPendingPost) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator animating={true} size="large" />
      </View>
    );
  }

  // Show error if request fails
  if (isError) {
    return (
      <View style={styles.centered}>
        <ErrorBlock>{error.message}</ErrorBlock>
      </View>
    );
  }

  // Render selected screen and pass down state to child components
  return (
    <>
      <ScrollView style={styles.rootContainer}>
        {errorMessage && <ErrorBlock>{errorMessage}</ErrorBlock>}
        {screen === "overview" && (
          <EditOverview
            title={title}
            description={description}
            handleTitleChange={handleTitleChange}
            handleDescriptionChange={handleDescriptionChange}
            hazards={hazards}
            selectedHazard={selectedHazard}
            handleSelectHazard={handleSelectHazard}
            setSelectedHazard={setSelectedHazard}
            handleAddHazard={handleAddHazard}
            handleUpdateHazard={handleUpdateHazard}
            handleRemoveHazard={handleRemoveHazard}
            version={version}
            status={status}
            isApproved={data?.isApproved}
            departments={departments}
            selectedDepartment={selectedDepartment}
            handleSelectDepartment={handleSelectDepartment}
          />
        )}

        {screen === "steps" && (
          <EditSteps
            steps={steps}
            setSteps={setSteps}
            ppeList={ppeList}
            handleEditStepPpe={handleEditStepPpe}
          />
        )}
      </ScrollView>
      <BottomBar selectedScreen={screen} onSelectScreen={selectScreen} />
    </>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
    margin: 20,
  },
  centered: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
});
