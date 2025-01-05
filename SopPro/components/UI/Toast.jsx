import Toast, { BaseToast, ErrorToast } from "react-native-toast-message";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTheme } from "react-native-paper";

export default function ToastWithInsets() {
  const insets = useSafeAreaInsets();
  const theme = useTheme();

  return (
    <Toast
      position="top"
      topOffset={insets.top}
      config={{
        success: (props) => (
          <BaseToast
            {...props}
            style={{
              borderLeftColor: "green",
              marginTop: insets.top,
            }}
          />
        ),
        error: (props) => (
          <ErrorToast
            {...props}
            style={{
              borderLeftColor: theme.colors.error,
              marginTop: insets.top,
            }}
          />
        ),
      }}
    />
  );
}
