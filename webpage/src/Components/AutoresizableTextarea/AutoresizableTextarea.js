import {useLayoutEffect, useRef} from "react";

const AutoresizableTextarea = ({className, value, onChange, placeholder}) => {

    const textareaRef = useRef();

    useLayoutEffect(() => {
        textareaRef.current.style.height = "15px";
        textareaRef.current.style.height = textareaRef.current.scrollHeight-4+'px';
    }, [value]);

    return (
        <textarea placeholder={placeholder} ref={textareaRef} className={className} value={value} onChange={onChange}/>
    )
};

export default AutoresizableTextarea;