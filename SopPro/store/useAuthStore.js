import { create } from "zustand";
import AsyncStorage from "@react-native-async-storage/async-storage";

const useAuthStore = create((set) => ({
  token: null,
  isLoggedIn: false,
  setToken: (token) => {  set({ token, isLoggedIn: !!token }) },
  logout: () => { set({ token: null, isLoggedIn: false }) }
}));

export default useAuthStore;