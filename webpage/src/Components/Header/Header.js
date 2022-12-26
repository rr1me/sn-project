import logo from './sn-logo.png';
import './Header.sass';
import {Link} from "react-router-dom";
import {useDispatch, useSelector} from "react-redux";
import {logoutRequest} from "../redux/authSlice";

const Header = () => {

    const {user} = useSelector(state=>state.authSlice);

    return (
        <div className='header'>
            <div className='actualHeader'>
                <img src={logo} className='logo'/>
                <div className='desc'>
                    <b>
                        100RAD <br/>
                        комплекс игровых серверов
                    </b>
                    <hr className='separator'/>
                    100RAD V 1.1 <br/>
                    (c)2022
                </div>
            </div>
            <div className='navigation'>
                <Link to='/'>главная</Link>
                <Link to='/banlist'>список блокировок</Link>
                <Link to='/123'>скачать клиент</Link>
                <Link to='#'>discord</Link>
                <PrivateAreaAndLogout user={user}/>
            </div>
        </div>
    )
};

const PrivateAreaAndLogout = ({user})=> {
    const dispatch = useDispatch();
    const logoutClick = () => dispatch(logoutRequest());

    return user ? (
            <>
                <Link to='/me'>{user.username}</Link>
                <Link to='/' onClick={logoutClick}>выход</Link>
            </>
        ) :
        <Link to='/login'>вход</Link>
}

export default Header;