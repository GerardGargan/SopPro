import * as React from 'react';
import { FAB, Portal } from 'react-native-paper';

const Fab = () => {
  const [state, setState] = React.useState({ open: false });

  const onStateChange = ({ open }) => setState({ open });

  const { open } = state;

  return (
      <Portal>
        <FAB.Group
          open={open}
          visible
          icon={open ? 'file-document' : 'plus'}
          style={{position: 'absolute', bottom: 50}}
          actions={[
            { icon: 'plus', label: 'Start from scratch', onPress: () => console.log('Pressed blank') },
            {
              icon: 'lightbulb',
              label: 'AI Generated',
              onPress: () => console.log('Pressed AI'),
            },
            {
              icon: 'image-multiple',
              label: 'Start with pictures',
              onPress: () => console.log('Pressed pictures'),
            },
          ]}
          onStateChange={onStateChange}
        />
      </Portal>
  );
};

export default Fab;