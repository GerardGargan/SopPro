import { StyleSheet, Text, View } from 'react-native'
import React from 'react'

const InputErrorMessage = ({ children }) => {
  return (
    <View style={styles.container}>
        <Text style={styles.error}>{children}</Text>
    </View>
  )
}

export default InputErrorMessage

const styles = StyleSheet.create({
    error: {
        color: 'red',
        fontSize: 14
    },
    container: {
        marginTop: 2
    }
})