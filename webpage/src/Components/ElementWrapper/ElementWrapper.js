import './ElementWrapper.sass';
import Header from "../Header/Header";
import {useEffect, useRef, useState} from "react";

const ElementWrapper = ({children}) => {
    const contentRef = useRef();
    const [height, setHeight] = useState('auto');

    useEffect(() => {
        if (contentRef.current){
            const v = contentRef.current.getBoundingClientRect().height;
            console.log(contentRef.current.getBoundingClientRect());
            // console.log(v);
            // console.log(v-(34 + 40));
            // setHeight(window.innerHeight);
        }
    }, []);

    return (
        <div className='elementWrapper'>
            <div className='content'>
                <Header/>
                {children}
                {/*<button onClick={() => {*/}
                {/*    console.log(contentRef.current.getBoundingClientRect())*/}
                {/*}}>press</button>*/}
            </div>
        </div>
    )
};

export default ElementWrapper;