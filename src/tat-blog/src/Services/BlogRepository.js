import axios from "axios";
import { get_api, post_api, put_api, delete_api } from './Methods';

export async function getPostsBlog(keywork = '',
    pageSize = 10, pageNumber = 1,
    sortColumn = '', sortOrder = '') {
    try {
        const response = await
            axios.get(`https://localhost:7126/api/posts/get-posts-filter?PageSize=${pageSize}&PageNumber=${pageNumber}&SortColumn=${sortColumn}
            &SortOrder=${sortOrder}`);

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

// export async function getAuthors() {
//     try {
//         const response = await
//             axios.get(`https://localhost:7126/api/authors`);

//         const data = response.data;
//         if (data.isSuccess)
//             return data.result;
//         else
//             return null;
//     } catch (error) {
//         console.log('Error', error.message);
//         return null;
//     }
// }

export function getAuthors() {
    return get_api(`https://localhost:7126/api/authors`);
}

export function getAuthor(slug ,
    pageSize = 10,
    pageNumber = 1,
    sortColumn = '',
    sortOrder = '') {
    return get_api(`https://localhost:7126/api/authors/${slug}/posts`);
}

export async function getAuthorById(id = 0) {
    if (id > 0)
        return get_api(`https://localhost:7126/api/authors/${id}`);
    return null;
}

export function addOrUpdateAuthor(formData) {
    return post_api(`https://localhost:7126/api/authors`,formData);
}

export function getCategories() {
    return get_api(`https://localhost:7126/api/categories`);
}

export async function getCategoryById(id = 0) {
    if (id > 0)
        return get_api(`https://localhost:7126/api/categories/${id}`);
    return null;
}

export function getCategory(slug) {
    return get_api(`https://localhost:7126/api/categories/${slug}/posts`);
}

export function addOrUpdateCategory(formData) {
    return post_api('https://localhost:7126/api/categories', formData);
}

export function getComments() {
    return get_api(`https://localhost:7126/api/comments`);
}

export async function getCommentById(id = 0) {
    if (id > 0)
        return get_api(`https://localhost:7126/api/comments/${id}`);
    return null;
}

export function deleteComment(formData) {
    return delete_api('https://localhost:7126/api/comments', formData);
}

export function getDashboard() {
    return get_api(`https://localhost:7126/api/dashboard`);
}


export function getTags() {
    return get_api(`https://localhost:7126/api/tags`);
}

export async function getTagById(id = 0) {
    if (id > 0)
        return get_api(`https://localhost:7126/api/tags/${id}`);
    return null;
}

export function getTag(slug) {
    return get_api(`https://localhost:7126/api/tags/${slug}/posts`);
}

export function addOrUpdateTag(formData) {
    return post_api('https://localhost:7126/api/tags', formData);
}

// export function getAuthors(slug = '') {
//     return get_api(`https://localhost:7126/api/authors/${slug}/posts`);
// }

export function getPosts(keyword = '',
    pageSize = 10,
    pageNumber = 1,
    sortColumn = '',
    sortOrder = '') {
    return get_api(`https://localhost:7126/api/posts/get-posts-filter?PageSize=${pageSize}& PageNumber=${pageNumber}& SortColumn=${sortColumn}& SortOrder=${sortOrder}`);
}

export function getPostsAdmin() {
    return get_api('https://localhost:7126/api/posts/get-filter');
}


export async function getPostById(id = 0) {
    if (id > 0)
        return get_api(`https://localhost:7126/api/posts/${id}`);
    return null;
}

export function addOrUpdatePost(formData) {
    return post_api('https://localhost:7126/api/posts', formData);
}

export function togglePost(id,formData) {
    return put_api(`https://localhost:7126/api/posts/${id}`, formData);
}

export async function getPost(slug) {
    return get_api(`https://localhost:7126/api/posts/byslug/${slug}`);
}

export function getPostsFilter(keyword = '',
    authorId = '',
    categoryId = '',
    year = '', month = '',
    pageSize = 10,
    pageNumber = 1,
    sortColumn = '',
    sortOrder = '') {
    let url = new URL('https://localhost:7126/api/posts/get-posts-filter'); 
    keyword !== '' && url.searchParams.append('Keyword', keyword); 
    authorId !== '' && url.searchParams.append('AuthorId', authorId); 
    categoryId !== '' && url.searchParams.append('CategoryId', categoryId); 
    year !== '' && url.searchParams.append('Year', year);
    month !== '' && url.searchParams.append('Month', month);
    sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn); 
    sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder); 
    url.searchParams.append('PageSize', pageSize);
    url.searchParams.append('PageNumber', pageNumber);
    return get_api(url.href);
}


// export async function getPost(slug) {
//     try {
//         const response = await
//             axios.get(`https://localhost:7126/api/posts/byslug/${slug}`);

//         const data = response.data;
//         if (data.isSuccess)
//             return data.result;
//         else
//             return null;
//     } catch (error) {
//         console.log('Error', error.message);
//         return null;
//     }

// }



