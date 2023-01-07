import {createAsyncThunk, createSlice} from '@reduxjs/toolkit'
import {login, logout} from "../../Services/authService";

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

const authSlice = createSlice({
    name: "auth",
    initialState: {
        user: null
    },
    reducers: {
        initState(state, action){
            state.user = action.payload;
        }
    },
    extraReducers: builder => {
        builder.addCase(loginRequest.fulfilled, (state, action) => {
            state.user = action.payload;
            localStorage.setItem('user', JSON.stringify(action.payload));
        })
            .addCase(logoutRequest.fulfilled, (state, action) => {
                state.user = null;
                localStorage.removeItem('user');
            })
    }
});
export default authSlice.reducer;
export const authActions = authSlice.actions;