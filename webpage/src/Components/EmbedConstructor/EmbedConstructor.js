import './EmbedConstructor.sass';
import {useRef, useState} from "react";
import AutoresizableTextarea from "../AutoresizableTextarea/AutoresizableTextarea";

const EmbedConstructor = () => {
    const messageRef = useRef();
    const [value, setValue] = useState('');

    return (
        <div className='embedConstructor'>
            <div className='message'>
                Сообщение
                <AutoresizableTextarea value={value} onChange={e => setValue(e.target.value)} className='textarea' type='text' ref={messageRef}/>
            </div>
        </div>
    )
};

export default EmbedConstructor;