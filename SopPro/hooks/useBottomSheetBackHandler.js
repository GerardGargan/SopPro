import { useCallback, useRef } from "react";
import { BackHandler } from "react-native";

/**
 * Hook that dismisses the bottom sheet on the hardware back button press if it is visible
 * @param {React.RefObject} bottomSheetRef - Ref to the bottom sheet which is going to be closed/dismissed on the back press
 */
export const useBottomSheetBackHandler = (bottomSheetRef) => {
  const backHandlerSubscriptionRef = useRef(null);

  const handleSheetPositionChange = useCallback(
    (index) => {
      const isBottomSheetVisible = index >= 0;

      if (isBottomSheetVisible && !backHandlerSubscriptionRef.current) {
        // Set up the back handler if the bottom sheet is visible
        backHandlerSubscriptionRef.current = BackHandler.addEventListener(
          "hardwareBackPress",
          () => {
            bottomSheetRef.current?.dismiss();
            return true;
          }
        );
      } else if (!isBottomSheetVisible) {
        backHandlerSubscriptionRef.current?.remove();
        backHandlerSubscriptionRef.current = null;
      }
    },
    [bottomSheetRef]
  );

  return { handleSheetPositionChange };
};
