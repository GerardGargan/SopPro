import { FlatList, ScrollView, StyleSheet, Text, View } from "react-native";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useSelector } from "react-redux";
import Header from "../../../components/UI/Header";
import { capitiliseFirstLetter } from "../../../util/validationHelpers";
import { fetchSops } from "../../../util/httpRequests";
import { useQuery } from "@tanstack/react-query";
import SopCardLarge from "../../../components/sops/SopCardLarge";
import SopCardLargeSkeleton from "../../../components/skeletons/SopCardLargeSkeleton";
import CustomBottomSheetModal from "../../../components/sops/bottomSheet/CustomBottomSheetModal";
import { useCallback, useRef, useState } from "react";
import LargeNoDataCard from "../../../components/favourites/LargeNoDataCard";
import SopHorizontalList from "../../../components/sops/SopHorizontalList";

const index = () => {
  const isFocused = useIsFocused();
  const name = useSelector((state) => state.auth.forename);
  const bottomSheetModalRef = useRef();

  const [bottomSheetSelectedSop, setBottomSheetSelectedSop] = useState(null);

  const { data, isFetching, isFetched, isError, error } = useQuery({
    queryKey: ["sops", "favourites"],
    queryFn: () => fetchSops({ isFavourite: true }),
  });

  const handlePresentModalPress = useCallback((sop) => {
    setBottomSheetSelectedSop(sop);
    bottomSheetModalRef.current?.present();
  }, []);

  return (
    <>
      <ScrollView style={styles.rootContainer}>
        <Header
          textStyle={{ color: "black" }}
          text={"Welcome " + capitiliseFirstLetter(name)}
        />

        <SopHorizontalList
          data={data}
          isFetched={isFetched}
          isFetching={isFetching}
          title="Favourites"
          handlePresentModalPress={handlePresentModalPress}
          EmptyCard={LargeNoDataCard}
          emptyDataName="favourites"
          buttonText="Browse SOPs"
          callbackRoute="sops"
          text="Add your most-used SOPs to Favourites for quick access. They'll appear right here on your home screen."
        />

        {isFocused && <Fab />}
      </ScrollView>

      <CustomBottomSheetModal
        ref={bottomSheetModalRef}
        sop={bottomSheetSelectedSop}
      />
    </>
  );
};

export default index;

const styles = StyleSheet.create({
  rootContainer: {
    flex: 1,
  },
});
