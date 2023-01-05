import {createAsyncThunk, createSlice} from "@reduxjs/toolkit";
import axios from "axios";

export const sendEmbedThunk = createAsyncThunk(
    'sendEmbed',
    async (_, {getState, rejectWithValue}) => {
        const state = getState()?.embedSlice
        console.log(state);

        const r = await axios.post('/api/bot/embed', state).catch(reason => console.log(reason));
        console.log(r);

    }
)

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
        color: '#00ff00',
        fields: [fieldTemplate()],
        timestamp: '',
        image: fourInputsFieldTemplate(false),
        thumbnail: fourInputsFieldTemplate(false),
        author: fourInputsFieldTemplate(true),
        footer:{
            text: '',
            icon_url: '',
            proxy_icon_url: ''
        }
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
            console.log(payload);
            const {index, type, value} = payload;
            state.fields[index][type] = value;
        },
        fourInputsReducer(state, {payload}){
            const {state:stateName, type, value} = payload;
            console.log(stateName, type, value);
            state[stateName][type] = value
        },
        setFooter(state, {payload}){
            const {type, value} = payload;
            state.footer[type] = value;
        }
    }
});

export default embedSlice.reducer;
export const embedActions = embedSlice.actions;