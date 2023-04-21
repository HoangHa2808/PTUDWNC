import React, { useEffect, useState } from 'react'
import Pager from '../Pager';
import PostItem from '../PostItem';

const PostSearch = ({postQuery}) => {
    const {
        keyword, year, month, tagSlug, authorSlug, categorySlug
    } = postQuery;

    const [pageNumber, setPageNumber] = useState(1);
    const [postsList, setPostsList] = useState({
        items: [],
        metadata: {}
    });

    useEffect(() => {
        loadBlogPosts();

        async function loadBlogPosts() {
            const parameters = new URLSearchParams({
                
                pageNumber: pageNumber || 1,
                pageSize: 10
            });

            const apiEndpoint = `https://localhost:7126/api/posts/get-posts-filter`;
            const response = await fetch(apiEndpoint);
            const data = await response.json();

            setPostsList(data.result);
            window.scrollTo(0, 0);
        }
    }, [keyword, year, month, tagSlug, authorSlug, categorySlug, pageNumber]);

    function updatePageNumber(inc) {
        setPageNumber((currentVal) => currentVal + inc);
    }

    return (
        <div className="posts-wrapper">
            <PostItem articles={postsList.items} />
            <Pager 
                pageCount={postsList.metadata.pageCount}
                hasNextPage={postsList.metadata.hasNextPage}
                hasPrevPage={postsList.metadata.hasPreviousPage}
                onPageChange={updatePageNumber} />
        </div>
    )
}

export default PostSearch;