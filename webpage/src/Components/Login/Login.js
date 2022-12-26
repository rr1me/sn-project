import './Login.sass'
import {useRef, useState} from "react";
import {authenticate} from "../../Services/authService";
import {useDispatch, useSelector} from "react-redux";
import {login} from "../redux/authSlice";
import {useNavigate} from "react-router-dom";

const Login = () => {
    const usernameRef = useRef();
    const passRef = useRef();

    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [error, setError] = useState();

    const {user} = useSelector(state => state.authSlice);

    const handleLoginButton = () => {
        const username = usernameRef.current.value;
        const password = passRef.current.value;
        console.log(username, password)
        authenticate(username, password).then(r => {
            dispatch(login(r.data))
            navigate("/");
        }).catch(r => setError(r.response.statusText));
    };

    return (
        <div className='login'>
            <input className='input' type='text' placeholder='Username' ref={usernameRef}/>
            <input className='input' type='text' placeholder='Password' ref={passRef}/>
            {error ? <h1>Error: {error}</h1> : null}
            <button className='button' onClick={handleLoginButton}>Войти</button>
        </div>
    )
};

export default Login;