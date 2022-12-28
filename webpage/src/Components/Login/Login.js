import './Login.sass'
import {useRef, useState} from "react";
import {useDispatch} from "react-redux";
import {loginRequest} from "../redux/authSlice";
import {useNavigate} from "react-router-dom";

const Login = () => {
    const usernameRef = useRef();
    const passRef = useRef();

    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [error, setError] = useState();

    const handleLoginButton = async () => {
        const username = usernameRef.current.value;
        const password = passRef.current.value;
        dispatch(loginRequest({username, password})).then(r => {
            if (r?.error)
                setError(r.error.message);
            else
                navigate('/')
        })
    };

    return (
        <div className='login'>
            <input className='input' type='text' placeholder='Username' ref={usernameRef}/>
            <input className='input' type='text' placeholder='Password' ref={passRef}/>
            <button className='button' onClick={handleLoginButton}>Войти</button>
            {error ? <b>Error: {error}</b> : null}
        </div>
    )
};

export default Login;