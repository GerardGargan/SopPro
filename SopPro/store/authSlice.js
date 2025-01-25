import { createSlice } from "@reduxjs/toolkit";
import AsyncStorage from "@react-native-async-storage/async-storage";

const authSlice = createSlice({
  name: "authSlice",
  initialState: {
    token: null,
    forename: null,
    surname: null,
    role: null,
    isLoggedIn: false,
  },
  reducers: {
    setToken(state, action) {
      state.token = action.payload;
      state.isLoggedIn = !!action.payload;
      AsyncStorage.setItem("authToken", action.payload);
    },
    setUserInfo(state, action) {
      state.forename = action.payload.forename;
      state.surname = action.payload.surname;
      state.role = action.payload.role;

      AsyncStorage.setItem("userInfo", JSON.stringify(action.payload));
    },
    logout(state) {
      state.token = null;
      state.isLoggedIn = false;
      state.forename = null;
      state.surname = null;
      state.role = null;

      AsyncStorage.removeItem("userInfo");
      AsyncStorage.removeItem("authToken");
    },
    initialiseAuth(state, action) {
      state.token = action.payload.token;
      state.isLoggedIn = !!action.payload.token;
      state.forename = action.payload.userInfo?.forename || null;
      state.surname = action.payload.userInfo?.surname || null;
      state.role = action.payload.userInfo?.role || null;
    },
  },
});

export const authActions = authSlice.actions;
export default authSlice.reducer;
