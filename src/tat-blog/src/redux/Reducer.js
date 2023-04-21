import { createSlice } from '@reduxjs/toolkit';
const initialState = {
    keyword: '',
    authorId: '',
    categoryId: '',
    year: '',
    month: ''
};
const postFilterReducer = createSlice({
    name: 'postFilter',
    initialState,
    reducers: {
        reset: (state, action) => {
            return initialState;
        },
        updateKeyword: (state, action) => {
            return {
                ...state,
                keyword: action.payload
            };
        },
        updateAuthorId: (state, action) => {
            return {
                ...state,
                authorId: action.payload
            };
        },
        updateCategoryId: (state, action) => {
            return {
                ...state,
                categoryId: action.payload
            };
        },
        updateMonth: (state, action) => {
            return {
                ...state,
                month: action.payload
            };
        },
        updateYear: (state, action) => {
            return {
                ...state,
                year: action.payload
            };
        },
    },
});
export const {
    reset,
    updateKeyword,
    updateAuthorId,
    updateCategoryId,
    updateMonth,
    updateYear } = postFilterReducer.actions;

export const reducer = postFilterReducer.reducer; 