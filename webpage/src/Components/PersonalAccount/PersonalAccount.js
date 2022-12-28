import './PersonalAccount.sass';
import {useSelector} from "react-redux";
import {Link} from "react-router-dom";

const PersonalAccount = () => {
    const {user} = useSelector(state => state.authSlice);

    return (
        <div className='personalAccount'>
            <div className='actions'>
                <Link to='/embed'>Create embed news</Link>
            </div>
            <div className='info'>
                здарова долбаеб
            </div>
        </div>
    )
};

export default PersonalAccount;