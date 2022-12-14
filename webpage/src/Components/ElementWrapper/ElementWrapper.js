import './ElementWrapper.sass';
import Header from "../Header/Header";

const ElementWrapper = ({children}) => {
    return (
        <div className='elementWrapper'>
            <Header/>
            {children}
        </div>
    )
};

export default ElementWrapper;