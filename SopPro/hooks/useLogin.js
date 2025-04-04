import { useMutation } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import { useDispatch } from "react-redux";
import { authActions } from "../store/authSlice";
import { login } from "../util/httpRequests";

const useLogin = () => {
  const router = useRouter();
  const dispatch = useDispatch();

  const { mutate, isPending, isError, error, data } = useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      const token = data?.result?.token;
      const refreshToken = data?.result?.refreshToken;
      const userInfo = {
        forename: data?.result?.forename,
        surname: data?.result?.surname,
        role: data?.result?.role,
      };

      if (token) {
        const payload = {
          token,
          refreshToken,
        };
        dispatch(authActions.setToken(payload));
        dispatch(authActions.setUserInfo(userInfo));

        router.replace("/(auth)");
      }
    },
  });

  return {
    mutate,
    isPending,
    isError,
    error,
    data,
  };
};

export default useLogin;
