import logo from './sn-logo.png';
import './Header.sass';

const Header = () => {
    return (
        <div className='header'>
            <div className='actualHeader'>
                <img src={logo} className='logo'/>
                <div>
                    complex of game servers and other bullshit
                </div>
            </div>
            <div className='navigation'>
                <div>гавная</div>
                <div>банхаммер</div>
                <div>клиент для клиентуры</div>
                <div>притон ебаный</div>
            </div>
        </div>
    )
};

export default Header;