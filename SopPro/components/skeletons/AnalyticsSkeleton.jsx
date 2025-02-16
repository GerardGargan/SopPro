import React from "react";
import {
  ScrollView,
  View,
  Text,
  StyleSheet,
  Dimensions,
  SafeAreaView,
} from "react-native";
import { SkeletonLoader } from "../UI/Skeleton";

const screenWidth = Dimensions.get("window").width;

const AnalyticsSkeleton = () => {
  const summaryCards = Array(4).fill(null);

  const SummaryCardSkeleton = () => (
    <View style={styles.card}>
      <SkeletonLoader height={14} style={styles.cardTitleSkeleton} />
      <SkeletonLoader height={24} style={styles.cardValueSkeleton} />
      <SkeletonLoader height={12} style={styles.cardSubtitleSkeleton} />
    </View>
  );

  const ChartCardSkeleton = ({ height }) => (
    <View style={styles.chartCard}>
      <SkeletonLoader height={16} style={styles.chartTitleSkeleton} />
      <SkeletonLoader height={height || 220} style={styles.chartSkeleton} />
    </View>
  );

  return (
    <SafeAreaView style={styles.safe}>
      <ScrollView style={styles.container}>
        <SkeletonLoader height={24} width={180} style={styles.headerSkeleton} />

        <View style={styles.summaryContainer}>
          {summaryCards.map((_, index) => (
            <SummaryCardSkeleton key={index} />
          ))}
        </View>

        <ChartCardSkeleton height={220} />

        <ChartCardSkeleton height={220} />

        <ChartCardSkeleton height={220} />

        <View style={{ marginVertical: 16 }}></View>
      </ScrollView>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
  container: {
    flex: 1,
    padding: 16,
  },
  headerSkeleton: {
    marginBottom: 16,
    borderRadius: 4,
  },
  summaryContainer: {
    flexDirection: "row",
    justifyContent: "space-between",
    flexWrap: "wrap",
    marginBottom: 16,
  },
  card: {
    backgroundColor: "white",
    borderRadius: 8,
    padding: 16,
    marginBottom: 12,
    width: "48%",
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 5,
  },
  cardTitleSkeleton: {
    width: "80%",
    borderRadius: 4,
  },
  cardValueSkeleton: {
    width: "60%",
    marginVertical: 8,
    borderRadius: 4,
  },
  cardSubtitleSkeleton: {
    width: "90%",
    borderRadius: 4,
  },
  chartCard: {
    backgroundColor: "white",
    borderRadius: 8,
    padding: 16,
    marginBottom: 16,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 5,
  },
  chartTitleSkeleton: {
    width: "50%",
    marginBottom: 16,
    borderRadius: 4,
  },
  chartSkeleton: {
    marginVertical: 8,
    borderRadius: 16,
    width: screenWidth - 64,
  },
});

export default AnalyticsSkeleton;
