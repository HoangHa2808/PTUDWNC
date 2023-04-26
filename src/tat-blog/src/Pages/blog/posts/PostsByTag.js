import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom';
import { isEmptyOrSpaces } from '../../../utils/Utils';
import { getTag } from '../../../services/BlogRepository';
import { Card } from 'react-bootstrap';
import TagList from '../../../components/blog/TagList';
// import PostSearch from '../components/blog/posts/PostSearch';

const PostsByTag = () => {
  const [tags, setTag] = useState([]);

  const { slug } = useParams();
  console.log("hi: ")
  console.log(slug)

  let imageUrl = isEmptyOrSpaces(tags.imageUrl)
    ? process.env.PUBLIC_URL + '/images/image_1.jpg'
    : `${tags.imageUrl}`;

  useEffect(() => {
    window.scrollTo(0, 0)
    getTag(slug).then(data => {
      console.log("data:")
      console.log(data)
      if (data)
        setTag(data);
      else
        setTag([]);
    });
  }, [slug]);

  return (
    <div>
      <h1>
        Articles Contain Tag <span className='text-primary'> {tags.urlSlug}</span>
      </h1> <div>
      <div className="card mb-5 ">

        <div className="row g-0">
          <div className="col-md-3">
            <Card.Img
              src={imageUrl}
              className="img-fluid rounded-start"
              alt={tags.title} />
          </div>
          <div className="col-md-9">
            <div className="card-body">
              <br />
              <small className='text-muted'>
                Tag:
              </small>
              <span
                className='text-decoration fw-bold'>
                {tags.urlSlug}
              </span>

              <div className='mb-3'>
                <TagList tags={tags.name} />
            </div>

              <p className='fw-bold text-success'>
                {tags.description}
              </p>

              <div
                className='post-content'
                dangerouslySetInnerHTML={{ __html: tags.description }}>
              </div>

            </div>
          </div>
        </div>
      </div>
    </div>
    </div>
  )
}

export default PostsByTag;