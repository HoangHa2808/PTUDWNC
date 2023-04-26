import axios from 'axios';

export async function get_api(your_api) {
    try {
        const response = await axios.get(your_api);
        const data = response.data;
        if (data.isSuccess)
            return data.result;
        else
            return null;
    } catch (error) {
        console.log('Error', error.message);
        return null;
    }
}

export async function post_api(your_api, formData) {
    try {
        const response = await axios.post(your_api, formData);
        const data = response.data;
        if (data.isSuccess)
            return data.result;
        else
            return null;
    } catch (error) {
        console.log('Error', error.message);
        return null;
    }
} 

export async function put_api(your_api, formData) {
    try {
        const response = await axios.put(your_api, formData);
        const data = response.data;
        if (data.isSuccess)
            return data.result;
        else
            return null;
    } catch (error) {
        console.log('Error', error.message);
        return null;
    }
} 
export async function delete_api(your_api, formData) {
    try {
        const response = await axios.delete(your_api, formData);
        const data = response.data;
        if (data.isSuccess)
            return data.result;
        else
            return null;
    } catch (error) {
        console.log('Error', error.message);
        return null;
    }
} 