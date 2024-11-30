import { createSlice } from "@reduxjs/toolkit";
import AsyncStorage from "@react-native-async-storage/async-storage";

const authSlice = createSlice({
  name: "authSlice",
  initialState: {
    token: null,
    isLoggedIn: false,
  },
  reducers: {
    setToken(state, action) {
        state.token = action.payload;
        state.isLoggedIn = !!action.payload;
        AsyncStorage.setItem("authToken", action.payload);
    },
    logout(state) {
        state.token = null;
        state.isLoggedIn = false;
        AsyncStorage.removeItem("authToken");
    },
    initialiseAuth(state, action) {
        state.token = action.payload;
        state.isLoggedIn = !!action.payload;
    }
  }
});

export const authActions = authSlice.actions;
export default authSlice.reducer;
