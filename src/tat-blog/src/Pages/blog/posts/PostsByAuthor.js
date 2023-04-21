import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom';
import PostsSearch from '../../../components/blog/posts/PostsSearch';
// import AuthorDetail from '../../blog/authors/AuthorDetail';

import { getAuthor } from "../../../services/BlogRepository";

const PostsByAuthor = () => {
    const [author, setAuthor] = useState([]);
    
    const { slug } = useParams();
    console.log("hi: ")
    console.log(slug)

    useEffect(() => {
        getAuthor(slug).then(data => {
            console.log("data:")
            console.log(data)
            if (data) {
                setAuthor(data);
            }
            else
                setAuthor([]);
        });
    }, [slug]);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [author]);

        return (

            <div className='p-4'>
                <h1 className='mb-4'>
                    Danh sách bài viết của tác giả: "{author.id}"
                </h1>
                <PostsSearch postQuery={{authorSlug: slug}} />
            </div>
        );
}

export default PostsByAuthor;