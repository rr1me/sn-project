import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import {login, logout, validateCredentials} from "../../Services/authService";

export const loginRequest = createAsyncThunk(
    'loginRequest',
    async (payload) => {
        const r = await login(payload).catch(r => {
            throw new Error(r.response.statusText)
        });
        return r.data;
    }
);

export const logoutRequest = createAsyncThunk(
    'logoutRequest',
    async () => {
        await logout()
    }
);

// export const validateCredentialsRequest = createAsyncThunk(
//     'validateCredentialsRequest',
//     async (_, {rejectWithValue}) => {
//         if (localStorage.getItem('user') !== null) {
//             const r = await validateCredentials();
//             console.log(r);
//             return;
//         }
//         return rejectWithValue();
//     }
// )

const authSlice = createSlice({
    name: "auth",
    initialState: {
        // user: {}
    },
    reducers: {
        initState(state, action){
            console.log(action);
            state.user = action.payload;
        }
        // login(state, action){
        //     const payload = action.payload;
        //     console.log(payload);
        //     state.user = action.payload;
        // },
        // logout(state){
        //     state.user = null;
        // }
    },
    extraReducers: builder => {
        builder.addCase(loginRequest.fulfilled, (state, action) => {
            state.user = action.payload;
        })
            .addCase(logoutRequest.fulfilled, (state, action) => {
                state.user = null;
            })
    }
});
export default authSlice.reducer;
export const authActions = authSlice.actions;
// export const {login, logout} = authSlice.actions;