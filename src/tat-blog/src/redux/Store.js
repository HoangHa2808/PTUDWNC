import { configureStore } from '@reduxjs/toolkit';
import { reducer } from './Reducer';
const store = configureStore({
    reducer: {
        postFilter: reducer,
    },
});
export default store; 