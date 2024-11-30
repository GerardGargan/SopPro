import { View, Text } from "react-native";
import React, { useEffect } from "react";
import { authActions } from "../../store/authSlice";
import { useDispatch } from "react-redux";

const logout = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(authActions.logout());
  }, [dispatch])
  
  return (
    <View>
      <Text>Logging out...</Text>
    </View>
  );
};

export default logout;
