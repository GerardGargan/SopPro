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

  const isCreate = id === "-1";

  const router = useRouter();
  const navigation = useNavigation();
  const queryClient = useQueryClient();

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

  const { data, isError, isFetching, isFetched, error } = useQuery({
    enabled: !isCreate,
    queryKey: ["sop", id],
    queryFn: () => fetchSop(id),
  });

  const { data: departmentsData } = useQuery({
    queryKey: ["departments"],
    queryFn: fetchDepartments,
  });

  const { data: ppeData } = useQuery({
    queryKey: ["ppe"],
    queryFn: fetchPpe,
  });

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

  function handleTitleChange(text) {
    setTitle(text);
  }

  function handleDescriptionChange(text) {
    setDescription(text);
  }

  function handleSave() {
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

  function handleSelectHazard(id) {
    setSelectedHazard(id);
  }

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

  function handleUpdateHazard(key, identifier, value) {
    setHazards((prevState) => {
      const hazards = [...prevState];
      const index = hazards.findIndex((hazard) => hazard.key === key);
      hazards[index][identifier] = value;
      return hazards;
    });
  }

  function handleRemoveHazard(key) {
    setHazards((prevState) => {
      return prevState.filter((hazard) => hazard.key !== key);
    });
    setSelectedHazard(null);
  }

  function handleSelectDepartment(departmentId) {
    if (departmentId === -1) {
      setSelectedDepartment(null);
    } else {
      setSelectedDepartment(departmentId);
    }
  }

  function selectScreen(screen) {
    setScreen(screen);
  }

  function getUpdateErrorMessage() {
    if (isErrorPut) return errorPut.message;
    if (isErrorPost) return errorPost.message;
    return null;
  }

  const errorMessage = getUpdateErrorMessage();

  if (isFetching || isPendingPut || isPendingPost) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator animating={true} size="large" />
      </View>
    );
  }

  if (isError) {
    return (
      <View style={styles.centered}>
        <ErrorBlock>{error.message}</ErrorBlock>
      </View>
    );
  }

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
          <EditSteps steps={steps} setSteps={setSteps} ppeList={ppeList} />
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
