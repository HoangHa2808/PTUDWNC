import axios from "axios";

export async function getCategories(){
    try{
        const response = await axios.get('https://localhost:7126/api/categories?PageSize=10&PageNumber=1&Paged=false');

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
        const response = await axios.get(`https://localhost:7126/api/tags?PageSize=10&PageNumber=1`);

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