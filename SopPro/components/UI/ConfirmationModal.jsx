import React from "react";
import { View } from "react-native";
import { Modal, Portal, Text, Button, Card } from "react-native-paper";

// Reusable confirmation prompt modal
const ConfirmationModal = ({
  visible,
  title,
  subtitle,
  onConfirm,
  onCancel,
}) => {
  return (
    <Portal>
      <Modal
        visible={visible}
        onDismiss={onCancel}
        contentContainerStyle={{ padding: 20 }}
      >
        <Card>
          <Card.Content>
            <Text variant="bodyLarge">{title}</Text>
            <Text variant="bodyMedium">{subtitle}</Text>
          </Card.Content>
          <Card.Actions>
            <Button onPress={onCancel}>No</Button>
            <Button onPress={onConfirm} mode="contained">
              Yes
            </Button>
          </Card.Actions>
        </Card>
      </Modal>
    </Portal>
  );
};

export default ConfirmationModal;
