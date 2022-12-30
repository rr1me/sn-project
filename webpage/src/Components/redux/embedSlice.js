import {createSlice} from "@reduxjs/toolkit";

const fieldTemplate = () => {
    return {
        id: Math.floor((Math.random() * 1000000)),
        title: '',
        text: '',
        inline: false
    }
}

const fourInputsFieldTemplate = isAuthor => {
    return isAuthor ?
        {
            name: '',
            url: '',
            icon_url: '',
            proxy_icon_url: ''
        } :
        {
            url: '',
            proxy_url: '',
            height: '',
            width: ''
        }
}

const embedSlice = createSlice({
    name: 'embedSlice',
    initialState:{
        title: '',
        description: '',
        url: '',
        color: '#ff0000',
        fields: [fieldTemplate()],
        timestamp: '',
        image: fourInputsFieldTemplate(false),
        thumbnail: fourInputsFieldTemplate(false),
        author: fourInputsFieldTemplate(true)
    },
    reducers: {
        setAnyField(state, {payload}){
            const {type, value} = payload;
            state[type] = value;
        },
        addField(state){
            state.fields.push(fieldTemplate());
        },
        removeField(state, {payload: id}){
            state.fields = state.fields.filter(v => v.id !== id);
        },
        setFieldBlock(state, {payload}){
            const {index, type, value} = payload;
            state.fields[index][type] = value;
        },
        setImageBlock(state, {payload}){
            const {type, value} = payload;
            state.image[type] = value;
        },
        fourInputsReducer(state, {payload}){
            const {state:stateName, type, value} = payload;
            console.log(stateName, type, value);
            state[stateName][type] = value
        }
    }
});

export default embedSlice.reducer;
export const embedActions = embedSlice.actions;