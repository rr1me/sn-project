import './EmbedConstructor.sass';
import {useMemo} from "react";
import AutoresizableTextarea from "../AutoresizableTextarea/AutoresizableTextarea";
import {useDispatch, useSelector} from "react-redux";
import {embedActions} from "../redux/embedSlice";

const {addField, removeField, setAnyField, setFieldBlock} = embedActions;

const EmbedConstructor = () => {
    const handleAnyField = type => e => dispatch(setAnyField({type:type, value:e.target.value}));

    const dispatch = useDispatch();
    const {title, description, url, color, fields} = useSelector(state => state.embedSlice);

    const handleFieldBlock = (index, type) => e => {
        dispatch(setFieldBlock({
            index: index,
            type: type,
            value: e.target.value
        }));
    };

    return (
        <div className='embedConstructor'>
            <div className='field'>
                Title
                {/*{useMemoArea(<AutoresizableTextarea value={title} onChange={handleAnyField('title')} className='textarea' type='text'/>, title)}*/}
                {useMemoArea(title, handleAnyField('title'), 'textarea')}
                Description
                {/*{useMemoArea(<AutoresizableTextarea value={description} onChange={handleAnyField('description')} className='textarea' type='text'/>, description)}*/}
                {useMemoArea(description, handleAnyField('description'), 'textarea')}
                URL
                {/*{useMemoArea(<AutoresizableTextarea value={url} onChange={handleAnyField('url')} className='textarea' type='text'/>, url)}*/}
                {useMemoArea(url, handleAnyField('url'), 'textarea')}
                Color
                <input className='colorPicker' type='color' value={color} onChange={handleAnyField('color')}/>
                Fields
                <div className='fields'>
                    <div className='tab'/>
                    <div className='actualFields'>
                        {fields.map((v,i) => {
                            return (
                                // <div className='fieldBlock' key={i}>
                                //     {useMemoArea('Field title', fields[i].title, handleFieldBlock(i, 'text'))}
                                //     {useMemoArea(<AutoresizableTextarea placeholder='Field text' value={fields[i].text} onChange={handleFieldBlock(i, 'text')}/>)}
                                //     <button>inline</button>
                                //     <button className='btn removeBtn' onClick={() => {
                                //         dispatch(removeField(v.id));
                                //     }}>x</button>
                                // </div>
                                <Field key={i} v={v} i={i} fields={fields} handleFieldBlock={handleFieldBlock} dispatch={dispatch}/>
                            )
                        })}
                        <button className='btn addBtn' onClick={() => {
                            dispatch(addField());
                        }}>Add field</button>
                    </div>
                </div>
            </div>
        </div>
    )
};

const useMemoArea = (value, onChange, className, placeholder) => useMemo(() => <AutoresizableTextarea className={className} placeholder={placeholder} value={value} onChange={onChange}/>, [value]);

const Field = ({v, i, fields, handleFieldBlock, dispatch}) =>
    <div className='fieldBlock' key={i}>
        {useMemoArea(fields[i].title, handleFieldBlock(i, 'title'), null, 'Field title')}
        {useMemoArea(fields[i].text, handleFieldBlock(i, 'text'), null, 'Field text')}
        {/*{useMemoArea(<AutoresizableTextarea placeholder='Field text' value={fields[i].text} onChange={handleFieldBlock(i, 'text')}/>)}*/}
        <button>inline</button>
        <button className='btn removeBtn' onClick={() => {
            dispatch(removeField(v.id));
        }}>x</button>
    </div>

export default EmbedConstructor;