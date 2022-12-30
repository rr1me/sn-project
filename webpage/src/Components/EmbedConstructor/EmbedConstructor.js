import './EmbedConstructor.sass';
import {memo, useMemo, useState} from "react";
import AutoresizableTextarea from "../AutoresizableTextarea/AutoresizableTextarea";
import {useDispatch, useSelector} from "react-redux";
import {embedActions} from "../redux/embedSlice";

const {addField, removeField, setAnyField, setFieldBlock, setImageBlock, fourInputsReducer} = embedActions;

const EmbedConstructor = () => {
    const handleAnyField = type => e => dispatch(setAnyField({type:type, value:e.target.value}));

    const dispatch = useDispatch();
    const {title, description, url, color, fields, timestamp, image, thumbnail, author} = useSelector(state => state.embedSlice);

    const addHandle = () => dispatch(addField());

    // const handleImageBlock = type => e => {
    //     dispatch(setImageBlock({
    //         type: type,
    //         value: e.target.value
    //     }))
    // }

    return (
        <div className='embedConstructor'>
            <div className='field'>
                Title
                {useMemoArea(title, handleAnyField('title'), 'textarea')}
                Description
                {useMemoArea(description, handleAnyField('description'), 'textarea')}
                URL
                {useMemoArea(url, handleAnyField('url'), 'textarea')}
                Color
                <input className='colorPicker' type='color' value={color} onChange={handleAnyField('color')}/>
                Fields
                <div className='fields'>
                    <div className='tab'/>
                    <div className='actualFields'>
                        {fields.map((v,i) => <Field key={i} id={v.id} index={i} title={v.title} text={v.text} inline={v.inline} dispatch={dispatch}/>)}
                        <button className='btn addBtn' onClick={addHandle}>Add field</button>
                    </div>
                </div>
                Timestamp
                <input type='text' value={timestamp} onChange={handleAnyField('timestamp')}/>
                Image
                <div className='image'>
                    <div className='tab'/>
                    {/*<div className='imageFields'>*/}
                    {/*    <input type='text' placeholder='URL' value={image.url} onChange={handleImageBlock('url')}/>*/}
                    {/*    <input type='text' placeholder='Proxy URL' value={image.proxy} onChange={handleImageBlock('proxy')}/>*/}
                    {/*    <input type='text' placeholder='Height' value={image.height} onChange={handleImageBlock('height')}/>*/}
                    {/*    <input type='text' placeholder='Width' value={image.width} onChange={handleImageBlock('width')}/>*/}
                    {/*</div>*/}
                    <FourInputsElement object={image} state={'image'} dispatch={dispatch}/>
                </div>
                Thumbnail
                <div className='thumbnail'>
                    <div className='tab'/>
                    <FourInputsElement object={thumbnail} state={'thumbnail'} dispatch={dispatch}/>
                </div>
                Author
                <div className='author'>
                    <div className='tab'/>
                    <FourInputsElement object={author} state={'author'} dispatch={dispatch} isAuthor={true}/>
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

    const handleFieldBlock = type => e => {
        dispatch(setFieldBlock({
            index: index,
            type: type,
            value: e.target.checked
        }));
    };

    return (
        <div className='fieldBlock'>
            {useMemoArea(title, handleFieldBlock('title'), null, 'Field title')}
            {useMemoArea(text, handleFieldBlock('text'), null, 'Field text')}
            <div className='inline'>
                <input type='checkbox' className='checkbox' checked={inline} onChange={handleFieldBlock('inline')}/>
                <label className='label'>Inline</label>
            </div>
            <button className='btn removeBtn' onClick={handleRemove}>x</button>
        </div>
    )
});

const useMemoArea = (value, onChange, className, placeholder) => useMemo(() => <AutoresizableTextarea className={className} placeholder={placeholder} value={value} onChange={onChange}/>, [value]);

export default EmbedConstructor;