// import { format } from 'date-fns';
import React, { useEffect, useMemo } from 'react'
import { useParams } from 'react-router-dom';
import PostSearch from '../../../components/blog/posts/PostsSearch';
import { isEmptyOrSpaces } from '../../../utils/Utils'

// import PostSearch from '../components/blog/posts/PostSearch';

const PostsByTime = () => {
  const params = useParams();
  let imageUrl = isEmptyOrSpaces(params.imageUrl)
  ? process.env.PUBLIC_URL + '/images/image_1.jpg'
  : `${params.imageUrl}`;

  const monthName = useMemo(() => {
    const year = +params.year || 0;
    const month = +params.month || 0;

    if (!year || year < 2000) return 'Unknown';
    if (!month || month < 1 || month > 12) return 'Unknown';

    const date = new Date(year, month, 1);
    // return format(date, 'MMMM yyyy');
  }, [params.year, params.month]);

  useEffect(() => {
    window.scrollTo(0, 0);
}, [params]);

  return (
    <div>
      <h1>
        Articles Published In <span className='text-primary'> {monthName}</span>
      </h1>

      {/* <PostSearch postQuery={{year: params.year, month: params.month}} /> */}
    </div>
  )
}

export default PostsByTime;