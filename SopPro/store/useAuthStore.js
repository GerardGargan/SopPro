import { create } from "zustand";
import AsyncStorage from "@react-native-async-storage/async-storage";

export const useAuthStore = create((set) => ({
  token: "",
  user: null,
  isAuthenticated: true,

  login: async (jwt, userData) => {
    await AsyncStorage.setItem("token", jwt);
    set({
      token: jwt,
      user: userData,
      isAuthenticated: true,
    });
  },

  logout: async () => {
    await AsyncStorage.removeItem("token");
    set({
      token: null,
      user: null,
      isAuthenticated: false,
    });
  },
}));
