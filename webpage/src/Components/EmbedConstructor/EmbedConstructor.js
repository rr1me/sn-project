import './EmbedConstructor.sass';
import {memo, useEffect, useMemo, useRef} from "react";
import AutoresizableTextarea from "../AutoresizableTextarea/AutoresizableTextarea";
import {useDispatch, useSelector} from "react-redux";
import {embedActions, sendEmbedThunk} from "../redux/embedSlice";

import avatar from './avatar.png';

const {addField, removeField, setAnyField, setFieldBlock, twoInputReducer} = embedActions;

const EmbedConstructor = () => {
    const previewRef = useRef();

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

    const {data: {title, description, url, color, fields, timestamp, image, thumbnail, author, footer}, message} = useSelector(state => state.embedSlice);

    const handleAnyField = type => e => dispatch(setAnyField({type:type, value:e.target.value}));

    const addHandle = () => dispatch(addField());

    const getFields = () => {
        return fields.map((v, i) => {
            return (
                <div className='embedField' key={i} style={v.inline ? {display: 'inline-block', paddingRight: 16} : null}>
                    <div className='title'>{v.title}</div>
                    <div className='text'>{v.text}</div>
                </div>
            )
        })
    };

    const sendBtnHandle = () => dispatch(sendEmbedThunk());

    const twoInputHandler = (state, type) => e => dispatch(twoInputReducer({
        state: state,
        type: type,
        value: type === 'isCurrent' ? e.target.checked : e.target.value
    }));

    return (
        <div className='embedConstructor'>

            <div className='field'>
                Title
                {useMemoArea(title, handleAnyField('title'), 'textarea')}
                Description
                {useMemoArea(description, handleAnyField('description'), 'textarea')}
                URL
                <input value={url} onChange={handleAnyField('url')}/>
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
                <input disabled={timestamp.isCurrent} type='text' placeholder='Ticks' value={timestamp.value} onChange={twoInputHandler('timestamp', 'value')}/>
                <div className='checkbox'>
                    <input type='checkbox' value={timestamp.isCurrent} onChange={twoInputHandler('timestamp', 'isCurrent')}/>
                    <label className='checkboxLabel'>Current timestamp</label>
                </div>
                Image URL
                <input type='text' value={image} onChange={handleAnyField('image')}/>
                Thumbnail URL
                <input type='text' value={thumbnail} onChange={handleAnyField('thumbnail')}/>
                Author
                <div className='author block'>
                    <div className='tab'/>
                    <div>
                        <input type='text' value={author.name} placeholder='Name' onChange={twoInputHandler('author', 'name')}/>
                        <input type='text' value={author.url} placeholder='Author URL' onChange={twoInputHandler('author', 'url')}/>
                        <input type='text' value={author.icon_url} placeholder='Icon URL' onChange={twoInputHandler('author', 'icon_url')}/>
                    </div>
                </div>
                Footer
                <div className='footer block'>
                    <div className='tab'/>
                    <div className='footerFields'>
                        {useMemoArea(footer.text, twoInputHandler('footer', 'text'), 'textarea', 'text')}
                        <input type='text' placeholder='Icon URL' value={footer.icon_url} onChange={twoInputHandler('footer', 'icon_url')}/>
                    </div>
                </div>
            </div>


            <div className='preview' ref={previewRef}>
                <div className='messageWrapper'>
                    <div className='message'>
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
                                    <div className='embedAuthor'>
                                        {author.icon_url ? <img src={author.icon_url} alt={author.proxy_icon_url} width={32} height={32}/> : null}
                                        <div className='authorName'>{author.url ? <a href={author.url}>{author.name}</a> : author.name}</div>
                                    </div>
                                    <div className='title'>{url ? <a href={url}>{title}</a> : title}</div>
                                    <div className='desc'>{description}</div>
                                    {getFields()}
                                    <div className='embedImage'>
                                        <img src={image.url} alt={image.proxy_url} width={image.width} height={image.height}/>
                                    </div>
                                    <div className='embedFooter'>
                                        {footer.icon_url ? <img src={footer.icon_url} alt={footer.proxy_icon_url} width={16} height={16}/> : null}
                                        {footer.text}
                                    </div>
                                </div>
                                <div className='embedThumbnail'>
                                    <img src={thumbnail.url} alt={thumbnail.proxy_url} width={thumbnail.width} height={thumbnail.height}/>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className='constructorHandle'>
                    <button className='btn sendBtn' onClick={sendBtnHandle}>Send news</button>
                    {message ? <b>{message}</b> : null}
                </div>

            </div>

        </div>
    )
};

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
            <div className='checkbox'>
                <input type='checkbox' className='checkboxInput' checked={inline} onChange={handleFieldBlock('inline', true)}/>
                <label className='checkboxLabel'>Inline</label>
            </div>
            <button className='btn removeBtn' onClick={handleRemove}>x</button>
        </div>
    )
});

const useMemoArea = (value, onChange, className, placeholder) => useMemo(() => <AutoresizableTextarea className={className} placeholder={placeholder} value={value} onChange={onChange}/>, [value]);

export default EmbedConstructor;