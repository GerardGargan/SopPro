import { useMutation } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import { useDispatch } from "react-redux";
import { authActions } from "../store/authSlice";
import { login } from "../util/httpRequests";

const useLogin = () => {
  const router = useRouter();
  const dispatch = useDispatch();

  const {
    mutate,
    isPending,
    isError,
    error,
    data
  } = useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      const token = data?.result?.token;

      if (token) {
        dispatch(authActions.setToken(token));
        router.navigate("/(auth)");
      }
    },
  });

  return {
    mutate,
    isPending,
    isError,
    error,
    data
  };
};

export default useLogin;