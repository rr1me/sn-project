import './ElementWrapper.sass';
import Header from "../Header/Header";
import {useLayoutEffect, useRef, useState} from "react";
import {useDispatch} from "react-redux";
import {validateCredentials} from "../../Services/authService";
import {authActions} from "../redux/authSlice";

const ElementWrapper = ({children}) => {
    const ref = useRef();
    const [style, setStyle] = useState('max-content');

    const dispatch = useDispatch();

    useLayoutEffect(() => {
        const user = JSON.parse(localStorage.getItem('user'));
        if (user !== null) validateCredentials().then(() => dispatch(authActions.initState(user))).catch(() => localStorage.removeItem('user'));

        setTimeout(() => {
            setStyle(ref?.current?.children[0].getBoundingClientRect().height);
        }, 50);
    }, []);

    return (
        <div className='elementWrapper' ref={ref} style={{height: style}}>
            <div className='content'>
                <Header/>
                {children}
            </div>
        </div>
    )
};

export default ElementWrapper;