import {configureStore} from "@reduxjs/toolkit";
import authSlice from "./authSlice";

const rehydrateStore = () => {
    if (localStorage.getItem('user') !== null) {
        return JSON.parse(localStorage.getItem('user')); // re-hydrate the store
    }
};

const store = configureStore({
    preloadedState: rehydrateStore(),
    reducer :{authSlice},
    // middleware: (defMiddleware) => defMiddleware.concat()
});

export default store;

const authMiddleware = store => next => action => {

}