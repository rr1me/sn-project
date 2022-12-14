import logo from './sn-logo.png';
import './Header.sass';

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
                <div>главная</div>
                <div>список блокировок</div>
                <div>скачать клиент</div>
                <div>discord</div>
            </div>
        </div>
    )
};

export default Header;