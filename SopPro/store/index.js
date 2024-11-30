import devToolsEnhancer from 'redux-devtools-expo-dev-plugin';
import { configureStore } from "@reduxjs/toolkit";
import authSliceReducer from './authSlice';

const store = configureStore({
    reducer: {
        auth: authSliceReducer
    },
    devTools: false,
    enhancers: getDefaultEnhancers => getDefaultEnhancers().concat(devToolsEnhancer())
});

export default store;