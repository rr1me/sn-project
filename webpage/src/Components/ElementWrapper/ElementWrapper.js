import './ElementWrapper.sass';
import Header from "../Header/Header";
import {useEffect, useLayoutEffect, useRef, useState} from "react";
import {useDispatch, useSelector} from "react-redux";
import {validateCredentials} from "../../Services/authService";
import {authActions} from "../redux/authSlice";
import {useNavigate} from "react-router-dom";

const ElementWrapper = ({authenticated, role, children}) => {
    const ref = useRef();
    const [style, setStyle] = useState('max-content');

    const dispatch = useDispatch();

    const navigate = useNavigate();
    const {user} = useSelector(state => state.authSlice);

    const initialized = useRef(false);
    useLayoutEffect(() => {
        const user = JSON.parse(localStorage.getItem('user'));
        if (user !== null) validateCredentials().then(() => dispatch(authActions.initState(user))).catch(() => localStorage.removeItem('user'));

        setTimeout(() => {
            setStyle(ref?.current?.children[0].getBoundingClientRect().height);
            initialized.current = true;
        }, 50);
    }, []);

    useEffect(() => {
        if (initialized?.current && authenticated && user === null) navigate('/login')
    });

    const content = (role === undefined || role === user?.role) ? children : 'Unauthorized';

    return (
        <div className='elementWrapper' ref={ref} style={{height: style}}>
            <div className='content'>
                <Header/>
                {content}
            </div>
        </div>
    )
};

export default ElementWrapper;