import {createAsyncThunk, createSlice} from "@reduxjs/toolkit";
import axios from "axios";

export const sendEmbedThunk = createAsyncThunk(
    'sendEmbed',
    async (_, {getState}) => {
        const state = getState()?.embedSlice.data;

        const [embed, error] = validateEmbed(state);

        if (error !== null) return error;

        const r = await axios.post('/api/bot/embed', embed).catch(r => r);

        if (r.status === 200)
            return 'Succeed';
        else
            return `Server error: ${r.response.status} - ${r.response.statusText}`;
    }
)

const validateFields = fields => {
    if (fields.length === 0)
        return [[], true]

    if (fields.length === 1){
        const {title, text} = fields[0];

        if (title === '' && text === '')
            return [[], true];

        if (title === '' || text === '')
            return [fields, false]

        return [fields, true];
    }

    let valid = true;

    let filteredFields = fields.filter(v => {
        if (v.title === '' && v.text === '')
            return false;

        if (v.title === '' || v.text === '')
            valid = false;

        return true;
    })

    return [filteredFields, valid];
}

const validateEmbed = (embed) => {
    let {title, description, fields:initFields, image, thumbnail, author, footer} = embed;
    const [fields, fieldCheck] = validateFields(initFields)

    if (title === '' && description === '' && fields.length === 0
        && image.url === '' && thumbnail.url === '' && author.name === ''
        && footer.text === '')
        return [[], "Embed message has to contain AT LEAST one element except for URL and timestamp. If its block element with few values, it has to have first input filled"];

    if (!fieldCheck)
        return [[], "Every field has to contain both title and text"];

    if (author.url !== ''){
        const r = /^(ftp|http|https):\/\/[^ "]+$/;
        if (!r.test(author.url))
            return [fields, "Not valid author URL"]
    }

    if (author.name === '' && (author.url !== '' || author.icon_url !== ''))
        return [[], "Author has to contain name if you want it element to show"];

    return [{...embed, fields: fields}, null];
}

const fieldTemplate = () => {
    return {
        id: Math.floor((Math.random() * 1000000)),
        title: '',
        text: '',
        inline: false
    }
}

// const fourInputsFieldTemplate = isAuthor => {
//     return isAuthor ?
//         {
//             name: '',
//             url: '',
//             icon_url: '',
//             proxy_icon_url: ''
//         } :
//         {
//             url: '',
//             proxy_url: '',
//             height: '',
//             width: ''
//         }
// }

const embedSlice = createSlice({
    name: 'embedSlice',
    initialState:{
        data:{
            title: '',
            description: '',
            url: '',
            color: '#00ff00',
            fields: [fieldTemplate()],
            timestamp: {
                isCurrent: false,
                value: ''
            },
            image: '',
            thumbnail: '',
            author: {
                name: '',
                url: '',
                icon_url: ''
            },
            footer:{
                text: '',
                icon_url: ''
            }
        },
        message: ''
    },
    reducers: {
        setAnyField(state, {payload}){
            const {type, value} = payload;
            state.data[type] = value;
        },
        addField(state){
            state.data.fields.push(fieldTemplate());
        },
        removeField(state, {payload: id}){
            state.data.fields = state.data.fields.filter(v => v.id !== id);
        },
        setFieldBlock(state, {payload}){
            const {index, type, value} = payload;
            state.data.fields[index][type] = value;
        },
        twoInputReducer(state, {payload}){
            const {state:stateName, type, value} = payload;
            state.data[stateName][type] = value
        }
    },
    extraReducers: builder => {
        builder.addCase(sendEmbedThunk.fulfilled, (state, action) => {
            state.message = action.payload;
        })
    }
});

export default embedSlice.reducer;
export const embedActions = embedSlice.actions;