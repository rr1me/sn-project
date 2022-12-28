import {configureStore} from "@reduxjs/toolkit";
import authSlice from "./authSlice";

const authMiddleware = store => next => action => {
    const f = '/fulfilled';
    const type = action.type;
    if (type === 'loginRequest'+f)
        localStorage.setItem('user', JSON.stringify(action.meta.arg));
    else if (type === 'logoutRequest'+f)
        localStorage.removeItem('user');

    return next(action)
};

const store = configureStore({
    reducer :{authSlice},
    middleware: (defMiddleware) => defMiddleware().concat(authMiddleware)
});

export default store;