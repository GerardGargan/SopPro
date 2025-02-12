import { View, Text } from "react-native";
import React from "react";

import { useEffect } from "react";
import { authActions } from "../store/authSlice";
import { useDispatch } from "react-redux";
import { Redirect } from "expo-router";
import { useQueryClient } from "@tanstack/react-query";

const logout = () => {
  const dispatch = useDispatch();
  const query = useQueryClient();
  useEffect(() => {
    query.clear();
    dispatch(authActions.logout());
  }, [dispatch, query]);
  return (
    <View>
      <Text>Logging out...</Text>
      <Redirect href="/home" />
    </View>
  );
};

export default logout;
