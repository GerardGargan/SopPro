import React from "react";
import {
  ScrollView,
  View,
  Text,
  StyleSheet,
  Dimensions,
  SafeAreaView,
} from "react-native";
import { LineChart, BarChart, PieChart } from "react-native-chart-kit";
import { useQuery } from "@tanstack/react-query";
import { getAnalytics } from "../../../util/httpRequests";
import AnalyticsSkeleton from "../../../components/skeletons/AnalyticsSkeleton";
import Toast from "react-native-toast-message";

const screenWidth = Dimensions.get("window").width;

const Analytics = () => {
  // Fetch analytics from API
  const { data, isPending, isError, error } = useQuery({
    queryKey: ["analytics", "sops"],
    queryFn: getAnalytics,
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
  });

  // Show error or Skeleton loader if loading
  if (isPending || isError) {
    if (isError) {
      Toast.show({
        type: "error",
        text1: error.message,
        visibilityTime: 3000,
      });
    }
    return <AnalyticsSkeleton />;
  }
  const chartConfig = {
    backgroundColor: "#ffffff",
    backgroundGradientFrom: "#ffffff",
    backgroundGradientTo: "#ffffff",
    decimalPlaces: 0,
    color: (opacity = 1) => `rgba(0, 136, 254, ${opacity})`,
    style: {
      borderRadius: 16,
    },
    propsForLabels: {
      fontSize: 10,
      rotation: -45,
    },
  };

  const lineData = {
    labels: data.lineData.labels,
    datasets: [
      {
        data: data.lineData.datasets[0].data,
        color: (opacity = 1) => `rgba(0, 136, 254, ${opacity})`,
        strokeWidth: 2,
      },
    ],
  };

  const barData = {
    labels: data.barData.labels,
    datasets: [
      {
        data: data.barData.datasets[0].data,
        color: (opacity = 1) => `rgba(0, 136, 254, ${opacity})`,
        strokeWidth: 2,
      },
    ],
  };

  const SummaryCard = ({ title, value, subtitle }) => (
    <View style={styles.card}>
      <Text style={styles.cardTitle}>{title}</Text>
      <Text style={styles.cardValue}>{value}</Text>
      <Text style={styles.cardSubtitle}>{subtitle}</Text>
    </View>
  );

  return (
    <SafeAreaView style={styles.safe}>
      <ScrollView style={styles.container}>
        <Text style={styles.header}>SOP Analytics</Text>

        <View style={styles.summaryContainer}>
          {data.summaryCards.map((item) => {
            return (
              <SummaryCard
                key={item.title}
                title={item.title}
                value={item.value}
                subtitle={item.subtitle}
              />
            );
          })}
        </View>

        <View style={styles.chartCard}>
          <Text style={styles.chartTitle}>Status Distribution</Text>
          <PieChart
            data={data.pieData}
            width={screenWidth - 32}
            height={220}
            chartConfig={chartConfig}
            accessor="population"
            backgroundColor="transparent"
            paddingLeft="15"
            absolute
          />
        </View>

        <View style={styles.chartCard}>
          <Text style={styles.chartTitle}>Monthly Activity</Text>
          <LineChart
            data={lineData}
            width={screenWidth - 64}
            height={220}
            chartConfig={chartConfig}
            bezier
            style={styles.chart}
            fromZero={true}
          />
        </View>

        <View style={styles.chartCard}>
          <Text style={styles.chartTitle}>SOPs by Department</Text>
          <BarChart
            data={barData}
            width={screenWidth - 64}
            height={220}
            chartConfig={chartConfig}
            style={styles.chart}
            showValuesOnTopOfBars
            fromZero={true}
          />
        </View>
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
  header: {
    fontSize: 24,
    fontWeight: "bold",
    marginBottom: 16,
    color: "#333",
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
  cardTitle: {
    fontSize: 14,
    fontWeight: "bold",
    color: "#666",
  },
  cardValue: {
    fontSize: 24,
    fontWeight: "bold",
    marginVertical: 8,
    color: "#333",
  },
  cardSubtitle: {
    fontSize: 12,
    color: "#666",
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
  chartTitle: {
    fontSize: 16,
    fontWeight: "bold",
    marginBottom: 16,
    color: "#333",
  },
  chart: {
    marginVertical: 8,
    borderRadius: 16,
  },
});

export default Analytics;
