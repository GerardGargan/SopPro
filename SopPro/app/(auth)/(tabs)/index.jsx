import { BackHandler, ScrollView, StyleSheet } from "react-native";
import Fab from "../../../components/sops/fab";
import { useIsFocused } from "@react-navigation/native";
import { useSelector } from "react-redux";
import Header from "../../../components/UI/Header";
import { capitiliseFirstLetter } from "../../../util/validationHelpers";
import { fetchSops } from "../../../util/httpRequests";
import { useQuery } from "@tanstack/react-query";
import CustomBottomSheetModal from "../../../components/sops/bottomSheet/CustomBottomSheetModal";
import { useCallback, useEffect, useRef, useState } from "react";
import LargeNoDataCard from "../../../components/favourites/LargeNoDataCard";
import SopHorizontalList from "../../../components/sops/SopHorizontalList";
import { Bookmark, Clock } from "lucide-react-native";
import { useRouter, useSegments } from "expo-router";

const index = () => {
  const isFocused = useIsFocused();
  const name = useSelector((state) => state.auth.forename);
  const bottomSheetModalRef = useRef();
  const segments = useSegments();

  const [bottomSheetSelectedSop, setBottomSheetSelectedSop] = useState(null);

  useEffect(() => {
    // Only prevent back button when this screen is focused
    const backHandler = BackHandler.addEventListener(
      "hardwareBackPress",
      () => {
        // Check if we're at the main tabs index screen
        if (
          segments.length == 2 &&
          segments[0] === "(auth)" &&
          segments[1] === "(tabs)"
        ) {
          // Prevent back navigation
          return true;
        }

        // Allow back button to be used for all other screens
        return false;
      }
    );

    // clean up event listener on unmount
    return () => backHandler.remove();
  }, [segments]);

  const {
    data: favouritesData,
    isPending: isFetchingFavourites,
    isFetched: isFetchedFavourites,
    isError: isErrorFavourites,
    error: erroFavourites,
  } = useQuery({
    queryKey: ["favourites", "sops"],
    queryFn: () => fetchSops({ isFavourite: true }),
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
  });

  const {
    data: recentData,
    isPending: isFetchingRecent,
    isFetched: isFetchedRecent,
    isError: isErrorRecent,
    error: errorRecent,
  } = useQuery({
    queryKey: ["recent", "sops"],
    queryFn: () => fetchSops({ pageSize: 10 }),
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
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
          data={favouritesData}
          isFetched={isFetchedFavourites}
          isFetching={isFetchingFavourites}
          isError={isErrorFavourites}
          title="Favourites"
          handlePresentModalPress={handlePresentModalPress}
          EmptyCard={LargeNoDataCard}
          emptyTitle="No favourites yet"
          buttonText="Browse SOPs"
          callbackRoute="sops"
          emptyText="Add your most-used SOPs to Favourites for quick access. They'll appear right here on your home screen."
          EmptyIcon={Bookmark}
        />

        <SopHorizontalList
          data={recentData}
          isFetched={isFetchedRecent}
          isFetching={isFetchingRecent}
          isError={isErrorRecent}
          title="Recently updated"
          handlePresentModalPress={handlePresentModalPress}
          EmptyCard={LargeNoDataCard}
          emptyTitle="Nothing to show yet!"
          buttonText="Create a SOP"
          callbackRoute="/upsert/-1"
          emptyText="Start creating and editing SOPs, the most recent edited SOPs in your organisation will appear here!"
          EmptyIcon={Clock}
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
