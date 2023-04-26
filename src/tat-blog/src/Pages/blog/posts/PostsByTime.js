// import { format } from 'date-fns';
import React, { useEffect, useMemo } from 'react'
import { useParams } from 'react-router-dom';
import PostSearch from '../../../components/blog/posts/PostsSearch';
import { isEmptyOrSpaces } from '../../../utils/Utils'
import { Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';

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
      <div className="card mb-5 ">

        <div className="row g-0">
          <div className="col-md-3">
            <Card.Img
              src={imageUrl}
              className="img-fluid rounded-start"
              alt={params.title} />
          </div>
          <div className="col-md-9">
            <div className="card-body">
              <div className="card-text">
                {/* <small className="text-muted">
                    Category: {post.categoryName}
                </small>
                <br/>
                <small className="text-muted">
                    Author: {post.authorName}
                </small> */}
                <br />
                <small className='text-muted'>
                  Published on:
                </small>
                <span className='fw-bold text-primary'>
                  {monthName}
                </span>
              </div>
              <br />
              <small className='text-muted'>
                By:
              </small>
              <span className='text-decoration-none fw-bold'>

                {params?.urlSlug}
              </span>

              <br />
              <small className='text-muted'>
                Category:
              </small>
              <Link
                className='text-decoration-none fw-bold'
                to={`/category/${params?.category?.urlSlug}`}>
                {params?.category?.name}
              </Link>

              <p className='fw-bold text-success'>
                {params?.shortDescription}
              </p>

              <div
                className='post-content'
                dangerouslySetInnerHTML={{ __html: params.description }}>
              </div>

            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default PostsByTime;