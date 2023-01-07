import {configureStore} from "@reduxjs/toolkit";
import authSlice from "./authSlice";
import embedSlice from "./embedSlice";

const store = configureStore({
    reducer :{authSlice, embedSlice}
});

export default store;