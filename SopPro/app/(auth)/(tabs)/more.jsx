import { StyleSheet, Text, View } from 'react-native'
import React from 'react'
import { Link } from 'expo-router'

const more = () => {
  return (
    <View style={{margin: 50}}>
      <Link href="logout">Logout</Link>
    </View>
  )
}

export default more

const styles = StyleSheet.create({})