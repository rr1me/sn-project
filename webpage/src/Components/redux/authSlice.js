import {createSlice} from "@reduxjs/toolkit";

const authSlice = createSlice({
    name: "auth",
    initialState: {
        user: {}
    },
    reducers: {
        login(state, action){
            const payload = action.payload;
            console.log(payload);
            state.user = action.payload;
        },
        logout(state){
            state.user = {};
        }
    }
});
export default authSlice.reducer;
export const {login, logout} = authSlice.actions;