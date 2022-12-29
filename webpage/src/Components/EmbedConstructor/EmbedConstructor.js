import './EmbedConstructor.sass';
import {memo, useMemo} from "react";
import AutoresizableTextarea from "../AutoresizableTextarea/AutoresizableTextarea";
import {useDispatch, useSelector} from "react-redux";
import {embedActions} from "../redux/embedSlice";

const {addField, removeField, setAnyField, setFieldBlock} = embedActions;

const EmbedConstructor = () => {
    const handleAnyField = type => e => dispatch(setAnyField({type:type, value:e.target.value}));

    const dispatch = useDispatch();
    const {title, description, url, color, fields} = useSelector(state => state.embedSlice);

    // const handleFieldBlock = (index, type) => e => {
    //     dispatch(setFieldBlock({
    //         index: index,
    //         type: type,
    //         value: e.target.value
    //     }));
    // };

    // const getFields = useCallback(() => fields.map((v,i) => <Field key={i} v={v} i={i} fields={fields} handleFieldBlock={handleFieldBlock} dispatch={dispatch}/>), [fields]);

    const addHandle = () => dispatch(addField());

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
                        {fields.map((v,i) => <Field key={i} id={v.id} index={i} title={v.title} text={v.text} dispatch={dispatch}/>)}
                        <button className='btn addBtn' onClick={addHandle}>Add field</button>
                    </div>
                </div>
            </div>
        </div>
    )
};

const Field = memo(({id, index, title, text, dispatch}) => {
    const handleRemove = () => dispatch(removeField(id));

    const handleFieldBlock = type => e => {
        dispatch(setFieldBlock({
            index: index,
            type: type,
            value: e.target.value
        }));
    };

    return (
        <div className='fieldBlock'>
            {useMemoArea(title, handleFieldBlock('title'), null, 'Field title')}
            {useMemoArea(text, handleFieldBlock('text'), null, 'Field text')}
            <button>inline</button>
            <button className='btn removeBtn' onClick={handleRemove}>x</button>
        </div>
    )
});

// const Fields = () => {
//     const {fields} = useSelector(state => state.embedSlice);
//
//     const dispatch = useDispatch();
//
//     const addHandle = () => dispatch(addField());
//
//     // const useField = () => useCallback(() =>{})
//
//     return (
//         <div className='fields'>
//             <div className='tab'/>
//             <div className='actualFields'>
//                 {fields.map((v,i) => <Field key={i} v={v} i={i} title={v.title} text={v.text} dispatch={dispatch}/>)}
//                 <button className='btn addBtn' onClick={addHandle}>Add field</button>
//             </div>
//         </div>
//     )
// }

const useMemoArea = (value, onChange, className, placeholder) => useMemo(() => <AutoresizableTextarea className={className} placeholder={placeholder} value={value} onChange={onChange}/>, [value]);

export default EmbedConstructor;