import logo from './sn-logo.png';
import './Header.sass';
import {Link} from "react-router-dom";

const Header = () => {
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
                <Link to='/12'>список блокировок</Link>
                <Link to='#'>скачать клиент</Link>
                <Link to='#'>discord</Link>
            </div>
        </div>
    )
};

export default Header;