import {configureStore, createStore} from "@reduxjs/toolkit";
import authSlice, {authActions} from "./authSlice";
import {validateCredentials} from "../../Services/authService";

const getPreloadedState = async () => {
    if (localStorage.getItem('user') !== null) {
        const r = await validateCredentials();
        console.log(r);


        const parse = JSON.parse(localStorage.getItem('user'));
        return {"authSlice": {user: parse}};
    }
};

const authMiddleware = store => next => action => {
    const f = '/fulfilled';
    const type = action.type;
    if (type === 'loginRequest'+f)
        localStorage.setItem('user', JSON.stringify(action.meta.arg));
    else if (type === 'logoutRequest'+f)
        localStorage.removeItem('user');

    return next(action)
};

// const loadMiddleware = store => {
//     const hl = () => {
//         // console.log(load)
//         console.log("eqwe")
//     }
//
//     return next => action => {
//         hl()
//         return next(action);
//     };
// }

const store = configureStore({
    // preloadedState: (async () => {
    //     await getPreloadedState();
    // }),
    // preloadedState: getPreloadedState(),
    reducer :{authSlice},
    middleware: (defMiddleware) => defMiddleware().concat(authMiddleware)
});

export default store;