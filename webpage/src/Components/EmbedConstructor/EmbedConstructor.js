import './EmbedConstructor.sass';
import {memo, useEffect, useMemo, useRef} from "react";
import AutoresizableTextarea from "../AutoresizableTextarea/AutoresizableTextarea";
import {useDispatch, useSelector} from "react-redux";
import {embedActions} from "../redux/embedSlice";

import avatar from './avatar.png';
import {Link} from "react-router-dom";

const {addField, removeField, setAnyField, setFieldBlock, fourInputsReducer, setFooter} = embedActions;

const EmbedConstructor = () => {
    const previewRef = useRef();
    const messageRef = useRef();

    useEffect(() => {
        const eventConsole = () => {
            const topPreview = previewRef.current?.parentElement.getBoundingClientRect().top
            if (topPreview < 35)
            {
                const data = -topPreview + 35;
                previewRef.current?.setAttribute('style', 'transform: translate3D(0, ' + data + 'px,0)')
            }else{
                previewRef.current?.setAttribute('style', 'transform: translate3D(0,0,0)')
            }
        };

        document.addEventListener('scroll', eventConsole);

        return () => document.removeEventListener('scroll', eventConsole);
    }, []);

    const dispatch = useDispatch();

    const {title, description, url, color, fields, timestamp, image, thumbnail, author, footer} = useSelector(state => state.embedSlice);

    const handleAnyField = type => e => dispatch(setAnyField({type:type, value:e.target.value}));

    const addHandle = () => dispatch(addField());

    const footerHandler = type => e => dispatch(setFooter({type:type, value:e.target.value}))

    const getFields = () => {
        return fields.map((v, i) => {
            console.log(v);
            return (
                <div className='embedField' key={i} style={v.inline ? {display: 'inline-block', paddingRight: 16} : null}>
                    <div className='title'>{v.title}</div>
                    <div className='text'>{v.text}</div>
                </div>
            )
        })
    }

    return (
        <div className='embedConstructor'>
            <div className='field'>
                Title
                {useMemoArea(title, handleAnyField('title'), 'textarea')}
                Description
                {useMemoArea(description, handleAnyField('description'), 'textarea')}
                URL
                {/*{useMemoArea(url, handleAnyField('url'), 'textarea')}*/}
                <input className='url' value={url} onChange={handleAnyField('url')}/>
                Color
                <input className='colorPicker' type='color' value={color} onChange={handleAnyField('color')}/>
                Fields
                <div className='fields block'>
                    <div className='tab'/>
                    <div className='actualFields'>
                        {fields.map((v,i) => <Field key={i} id={v.id} index={i} title={v.title} text={v.text} inline={v.inline} dispatch={dispatch}/>)}
                        <button className='btn addBtn' onClick={addHandle}>Add field</button>
                    </div>
                </div>
                Timestamp
                <input type='text' value={timestamp} onChange={handleAnyField('timestamp')}/>
                Image
                <div className='image block'>
                    <div className='tab'/>
                    <FourInputsElement object={image} state={'image'} dispatch={dispatch}/>
                </div>
                Thumbnail
                <div className='thumbnail block'>
                    <div className='tab'/>
                    <FourInputsElement object={thumbnail} state={'thumbnail'} dispatch={dispatch}/>
                </div>
                Author
                <div className='author block'>
                    <div className='tab'/>
                    <FourInputsElement object={author} state={'author'} dispatch={dispatch} isAuthor={true}/>
                </div>
                Footer
                <div className='footer block'>
                    <div className='tab'/>
                    <div className='footerFields'>
                        {useMemoArea(footer.text, footerHandler('text'), 'textarea', 'text')}
                        <input type='text' placeholder='Icon URL' value={footer.icon_url} onChange={footerHandler('icon_url')}/>
                        <input type='text' placeholder='Proxy icon URL' value={footer.proxy_icon_url} onChange={footerHandler('proxy_icon_url')}/>
                    </div>
                </div>
            </div>
            <div className='preview' ref={previewRef}>
                <div className='message' ref={messageRef}>
                    <div className='avatar'>
                        <img src={avatar}/>
                    </div>
                    <div className='bot'>
                        <div className='name'>
                            <b>100RAD</b>
                            <div className='tag'>BOT</div>
                            <div className='date'>Never</div>
                        </div>
                        <div className='embed' style={{borderColor: color}}>
                            <div className='embedContent'>
                                <div className='title'>{url ? <a href={url}>{title}</a> : title}</div>
                                <div className='desc'>{description}</div>
                                {getFields()}
                                <div className='embedImage'>
                                    <img src={image.url} alt={image.proxy_url} width={image.width} height={image.height}/>
                                </div>
                            </div>
                            <div className='embedThumbnail'>
                                <img src={thumbnail.url} alt={thumbnail.proxy_url} width={thumbnail.width} height={thumbnail.height}/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
};

const FourInputsElement = memo(({object, state, dispatch, isAuthor}) => {
    const FourInputsHandler = type => e => {
        dispatch(fourInputsReducer({
            state: state,
            type: type,
            value: e.target.value
        }))
    }

    return (
        <div className='imageFields'>
            <input type='text' placeholder={isAuthor ? 'Name' : 'URL'} value={object[isAuthor ? 'name' : 'url']} onChange={FourInputsHandler(isAuthor ? 'name' : 'url')}/>
            <input type='text' placeholder={isAuthor ? 'URL' : 'Proxy URL'} value={object[isAuthor ? 'url' : 'proxy_url']} onChange={FourInputsHandler(isAuthor ? 'url' : 'proxy_url')}/>
            <input type='text' placeholder={isAuthor ? 'Icon URL' : 'Height'} value={object[isAuthor ? 'icon_url' : 'height']} onChange={FourInputsHandler(isAuthor ? 'icon_url' : 'height')}/>
            <input type='text' placeholder={isAuthor ? 'Proxy icon URL' : 'Width'} value={object[isAuthor ? 'proxy_icon_url' : 'width']} onChange={FourInputsHandler(isAuthor ? 'proxy_icon_url' : 'width')}/>
        </div>
    )
});

const Field = memo(({id, index, title, text, inline, dispatch}) => {
    const handleRemove = () => dispatch(removeField(id));

    const handleFieldBlock = (type, isCheckbox) => e => {
        dispatch(setFieldBlock({
            index: index,
            type: type,
            value: isCheckbox ? e.target.checked : e.target.value
        }));
    };

    return (
        <div className='fieldBlock'>
            {useMemoArea(title, handleFieldBlock('title'), null, 'Field title')}
            {useMemoArea(text, handleFieldBlock('text'), null, 'Field text')}
            <div className='inline'>
                <input type='checkbox' className='checkbox' checked={inline} onChange={handleFieldBlock('inline', true)}/>
                <label className='label'>Inline</label>
            </div>
            <button className='btn removeBtn' onClick={handleRemove}>x</button>
        </div>
    )
});

const useMemoArea = (value, onChange, className, placeholder) => useMemo(() => <AutoresizableTextarea className={className} placeholder={placeholder} value={value} onChange={onChange}/>, [value]);

export default EmbedConstructor;