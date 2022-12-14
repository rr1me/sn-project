import './ElementWrapper.sass';
import Header from "../Header/Header";

const ElementWrapper = ({children}) => {
    return (
        <div className='elementWrapper'>
            <div className='content'>
                <Header/>
                {children}
            </div>
        </div>
    )
};

export default ElementWrapper;