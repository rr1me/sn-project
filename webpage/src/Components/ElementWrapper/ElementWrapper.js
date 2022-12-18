import './ElementWrapper.sass';
import Header from "../Header/Header";
import {useEffect, useLayoutEffect, useRef, useState} from "react";

const ElementWrapper = ({children}) => {
    const ref = useRef();
    const [style, setStyle] = useState('max-content');

    useLayoutEffect(() => {
        setTimeout(() => {
            setStyle(ref?.current?.children[0].getBoundingClientRect().height);
        }, 50);
    }, []);

    return (
        <div className='elementWrapper' ref={ref} style={{height: style}}>
            <div className='content'>
                <Header/>
                {children}
            </div>
        </div>
    )
};

export default ElementWrapper;