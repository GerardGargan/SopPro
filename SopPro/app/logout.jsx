import { View, Text } from 'react-native'
import React from 'react'

import { useEffect } from 'react'
import { authActions } from '../store/authSlice';
import { useDispatch } from 'react-redux';
import { Redirect } from 'expo-router';

const logout = () => {

    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(authActions.logout());
    }, [dispatch])
  return (
    <View>
      <Text>Logging out...</Text>
      <Redirect href="/home" />
    </View>
  )
}

export default logout