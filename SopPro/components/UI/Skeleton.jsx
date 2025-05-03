import React, { useEffect, useRef } from "react";
import { View, Animated, StyleSheet, Dimensions } from "react-native";

const SkeletonLoader = ({
  width = Dimensions.get("window").width,
  height = 20,
  style,
}) => {
  const animatedValue = useRef(new Animated.Value(1)).current;

  useEffect(() => {
    const shimmer = Animated.loop(
      Animated.sequence([
        Animated.timing(animatedValue, {
          toValue: 1,
          duration: 1000,
          useNativeDriver: true,
        }),
        Animated.timing(animatedValue, {
          toValue: 0,
          duration: 1000,
          useNativeDriver: true,
        }),
      ])
    );

    shimmer.start();

    return () => shimmer.stop();
  }, []);

  const translateX = animatedValue.interpolate({
    inputRange: [0, 1],
    outputRange: [-width, width],
  });

  return (
    <View style={[styles.container, { width, height }, style]}>
      <Animated.View
        style={[
          styles.shimmer,
          {
            transform: [{ translateX }],
          },
        ]}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: "#E1E9EE",
    overflow: "hidden",
    borderRadius: 4,
  },
  shimmer: {
    width: "100%",
    height: "100%",
    position: "absolute",
    backgroundColor: "#F2F8FC",
    transform: [{ translateX: 0 }],
  },
});

export { SkeletonLoader };
