import {createSlice} from "@reduxjs/toolkit";

const fieldTemplate = () => {
    return {
        id: Math.floor((Math.random() * 1000000)),
        title: '',
        text: '',
        inline: false
    }
}

const embedSlice = createSlice({
    name: 'embedSlice',
    initialState:{
        title: '',
        description: '',
        url: '',
        color: '#ff0000',
        fields: [fieldTemplate()]
    },
    reducers: {
        setAnyField(state, {payload}){
            const {type, value} = payload;
            state[type] = value;
        },
        addField(state){
            state.fields.push(fieldTemplate())
        },
        removeField(state, {payload: id}){
            state.fields = state.fields.filter(v => v.id !== id);
        },
        setFieldBlock(state, {payload}){
            const {index, type, value} = payload
            state.fields[index][type] = value
        }
    }
});

export default embedSlice.reducer;
export const embedActions = embedSlice.actions;