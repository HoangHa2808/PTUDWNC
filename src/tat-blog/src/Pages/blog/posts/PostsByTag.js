import React, { useEffect } from 'react'
import { useParams } from 'react-router-dom';
// import PostSearch from '../components/blog/posts/PostSearch';

const PostsByTag = () => {
  const params = useParams();

  useEffect(() => {
    window.scrollTo(0, 0);
}, [params]);

  return (
    <div>
      <h1>
        Articles Contain Tag <span className='text-primary'> {params.slug}</span>
      </h1>

      {/* <PostSearch postQuery={{tagSlug: params.slug}} /> */}
    </div>
  )
}

export default PostsByTag;