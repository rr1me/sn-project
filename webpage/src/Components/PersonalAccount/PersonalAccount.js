import './PersonalAccount.sass';
import {useSelector} from "react-redux";
import {useNavigate} from "react-router-dom";

const PersonalAccount = () => {
    const navigate = useNavigate();
    const {user} = useSelector(state => state.authSlice);

    const handleNavigateButton = link => e => navigate(link);

    return (
        <div className='personalAccount'>
            <div className='actions'>
                {user.role === 'Admin' ? <button className='btn linkBtn' onClick={handleNavigateButton('/embed')}>Create embed news</button> : null}
            </div>
            <div className='info'>
                o hello there
            </div>
        </div>
    )
};

export default PersonalAccount;