import axios from "axios";
import { get_api } from './Methods';  

// export async function getCategories(){
//     try{
//         const response = await axios.get('https://localhost:7126/api/categories');

//         const data = response.data;
//         if (data.isSuccess)           
//             return data.result;
//             else
//             return null;

//     } catch (error) {
//         console.log('Error', error.message);
//         return null;
//     }
// }

export function getCategories() { 
    return get_api(`https://localhost:7126/api/categories`); 
}  
   
export async function getBestAuthors(limit){
    try{
        const response = await axios.get(`https://localhost:7126/api/authors/best/${limit}`);

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

export async function getFeaturedPosts(limit){
    try{
        const response = await axios.get(`https://localhost:7126/api/posts/featured/${limit}`);

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

export async function getRandomPosts(limit){
    try{
        const response = await axios.get(`https://localhost:7126/api/posts/random/${limit}`);

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

export async function getArchives(limit){
    try{
        const response = await axios.get(`https://localhost:7126/api/posts/archives/${limit}`);

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

export async function getTagCloud(){
    try{
        const response = await axios.get(`https://localhost:7126/api/tags`);

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