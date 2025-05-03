import { StyleSheet, Text, View } from "react-native";
import MenuCard from "../UI/MenuCard";

// Reusable component for displaying a list of MenuCards with functions to run on selection
const MenuSection = ({ title, options }) => (
  <View style={styles.section}>
    <View style={styles.sectionHeader}>
      <Text variant="titleMedium" style={styles.sectionTitle}>
        {title}
      </Text>
      <View style={styles.divider} />
    </View>
    <View style={styles.menuItems}>
      {options.map((option) => (
        <MenuCard
          key={option.key}
          title={option.title}
          Icon={option.icon}
          onPress={option.onPress}
        />
      ))}
    </View>
  </View>
);

const styles = StyleSheet.create({
  sectionHeader: {
    gap: 8,
  },
  sectionTitle: {
    fontWeight: "600",
  },
  divider: {
    height: 1,
    backgroundColor: "#e0e0e0",
  },
  menuItems: {
    gap: 8,
  },
});

export default MenuSection;
