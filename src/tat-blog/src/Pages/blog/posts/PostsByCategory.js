import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom';
import { isEmptyOrSpaces } from '../../../utils/Utils';
import { getCategory } from '../../../services/BlogRepository';
import { Card } from 'react-bootstrap';
import { Link } from 'react-router-dom';
// import PostSearch from '../components/blog/posts/PostSearch';

const PostsByCategory = () => {
  const [category, setCategory] = useState([]);

  const { slug } = useParams();
  console.log("hi: ")
  console.log(slug)

  let imageUrl = isEmptyOrSpaces(category.imageUrl)
    ? process.env.PUBLIC_URL + '/images/image_1.jpg'
    : `${category.imageUrl}`;

  let postedDate = new Date(category.postedDate);


  useEffect(() => {
    window.scrollTo(0, 0)
    getCategory(slug).then(data => {
      console.log("data:")
      console.log(data)
      if (data)
        setCategory(data);
      else
        setCategory([]);
    });
  }, [slug]);

  return (
    <div>
      <div className="card mb-5 ">

        <div className="row g-0">
          <div className="col-md-3">
            <Card.Img
              src={imageUrl}
              className="img-fluid rounded-start"
              alt={category.title} />
          </div>
          <div className="col-md-9">
            <div className="card-body">
              <br />
              <small className='text-muted'>
                Category:
              </small>
              <span
                className='text-decoration fw-bold'>
                {category.urlSlug}
              </span>

              {/* <div className='mb-3'>
                <TagList tags={category.tags} />
            </div> */}

              <p className='fw-bold text-success'>
                {category?.shortDescription}
              </p>

              <div
                className='post-content'
                dangerouslySetInnerHTML={{ __html: category.description }}>
              </div>

            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default PostsByCategory;