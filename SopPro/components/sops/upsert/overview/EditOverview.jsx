import { StyleSheet, View } from "react-native";
import { Chip } from "react-native-paper";
import Header from "../../../UI/Header";
import SopForm from "./SopForm";
import HazardSection from "./HazardSection";
import { ScrollView } from "react-native-gesture-handler";

// Helper function to transform the Enum to a text description for Status
function getStatus(identifier) {
  switch (identifier) {
    case 1:
      return "Draft";
    case 2:
      return "In review";
    case 3:
      return "Approved";
    case 4:
      return "Archived";
    case 5:
      return "Rejected";
    default:
      return "Unknown";
  }
}

// Shows the overview form for a blank or existing SOP
const EditOverview = ({
  title,
  description,
  hazards,
  handleTitleChange,
  handleDescriptionChange,
  handleSelectHazard,
  setSelectedHazard,
  handleAddHazard,
  selectedHazard,
  handleUpdateHazard,
  handleRemoveHazard,
  version,
  isApproved,
  status,
  departments,
  selectedDepartment,
  handleSelectDepartment,
}) => {
  let versionChip = (
    <Chip
      icon="information-outline"
      mode="outlined"
      style={{ alignSelf: "flex-end" }}
    >
      Version {version}
    </Chip>
  );

  let newVersionChip = (
    <Chip icon="information-outline" mode="outlined">
      A new version will be created (V{version + 1})
    </Chip>
  );
  let statusChip = <Chip>{getStatus(status)}</Chip>;

  return (
    <>
      <ScrollView
        contentContainerStyle={styles.chipContainer}
        horizontal
        showsHorizontalScrollIndicator={false}
      >
        {statusChip}
        {isApproved ? newVersionChip : versionChip}
      </ScrollView>
      <SopForm
        selectedDepartment={selectedDepartment}
        departments={departments}
        handleSelectDepartment={handleSelectDepartment}
        handleDescriptionChange={handleDescriptionChange}
        handleTitleChange={handleTitleChange}
        title={title}
        description={description}
      />
      <Header text="Safety information" textStyle={{ color: "black" }} />
      <HazardSection
        hazards={hazards}
        selectedHazard={selectedHazard}
        setSelectedHazard={setSelectedHazard}
        handleSelectHazard={handleSelectHazard}
        handleAddHazard={handleAddHazard}
        handleRemoveHazard={handleRemoveHazard}
        handleUpdateHazard={handleUpdateHazard}
      />
    </>
  );
};

export default EditOverview;

const styles = StyleSheet.create({
  chipContainer: {
    flexDirection: "row",
    paddingHorizontal: 10,
    gap: 10,
    marginBottom: 10,
    justifyContent: "flex-end",
    minWidth: "100%",
  },
});
