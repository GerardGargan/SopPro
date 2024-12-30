import { ScrollView, StyleSheet, Text, View } from "react-native";
import React, { useEffect, useLayoutEffect } from "react";
import { Button, ActivityIndicator } from "react-native-paper";
import { useMutation, useQuery } from "@tanstack/react-query";
import { useLocalSearchParams, useNavigation, useRouter } from "expo-router";
import { createSop, fetchSop, updateSop } from "../../../util/httpRequests";
import EditOverview from "../../../components/sops/upsert/EditOverview";
import BottomBar from "../../../components/sops/upsert/BottomBar";
import EditSteps from "../../../components/sops/upsert/EditSteps";

const Upsert = () => {
  const { id } = useLocalSearchParams();

  const [title, setTitle] = React.useState("");
  const [description, setDescription] = React.useState("");
  const [hazards, setHazards] = React.useState([]);
  const [steps, setSteps] = React.useState([]);
  const [selectedHazard, setSelectedHazard] = React.useState(null);
  const [screen, setScreen] = React.useState("overview");
  const [version, setVersion] = React.useState(1);
  const [status, setStatus] = React.useState(1);

  const isCreate = id === "-1";

  const router = useRouter();
  const navigation = useNavigation();
  useLayoutEffect(() => {
    navigation.setOptions({
      title: isCreate ? "Create SOP" : "Edit SOP",
      headerRight: () => (
        <Button onPressIn={handleSave}>{isCreate ? "Save" : "Update"}</Button>
      ),
    });
  }, [navigation, handleSave]);

  const { data, isError, isLoading } = useQuery({
    enabled: !isCreate,
    queryKey: ["sop", id],
    queryFn: () => fetchSop(id),
  });

  const { mutate: mutateUpdate } = useMutation({
    mutationFn: updateSop,
    onSuccess: () => {
      router.navigate("/(auth)");
    },
  });

  const { mutate: mutateCreate } = useMutation({
    mutationFn: createSop,
    onSuccess: () => {
      router.navigate("/(auth)");
    },
  });

  useEffect(() => {
    if (data) {
      setTitle(data?.title || "");
      setDescription(data?.description || "");
      setHazards(
        data?.sopHazards.map((hazard) => {
          return { ...hazard, key: hazard.id };
        }) || []
      );
      setStatus(data?.status || 1);
      setVersion(data?.version || 1);
      setSteps(data?.sopSteps || []);
    }
  }, [data]);

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

  function selectScreen(screen) {
    setScreen(screen);
  }

  if (isLoading) {
    return <ActivityIndicator animating={true} />;
  }

  if (isError) {
    return <Text>Error fetching SOP</Text>;
  }

  return (
    <>
      <ScrollView style={styles.rootContainer}>
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
          />
        )}

        {screen === "steps" && <EditSteps steps={steps} />}
      </ScrollView>
      <BottomBar selectedScreen={screen} onSelectScreen={selectScreen} />
    </>
  );
};

export default Upsert;

const styles = StyleSheet.create({
  rootContainer: {
    margin: 20,
  },
});
