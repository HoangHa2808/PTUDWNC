import React, { useEffect, useState, } from 'react'
// import AuthorDetail from '../../blog/authors/AuthorDetail';

import { getAuthor, } from "../../../services/BlogRepository";
import { Card } from 'react-bootstrap';
import { isEmptyOrSpaces } from '../../../utils/Utils';
import { Link, useParams } from 'react-router-dom';

const PostsByAuthor = () => {
    const [author, setAuthor] = useState([]);

    const { slug } = useParams();
    console.log("hi: ")
    console.log(slug)

    let imageUrl = isEmptyOrSpaces(author.imageUrl)
        ? process.env.PUBLIC_URL + '/images/image_1.jpg'
        : `${author.imageUrl}`;

    let postedDate = new Date(author.postedDate);


    useEffect(() => {
        window.scrollTo(0, 0)
        getAuthor(slug).then(data => {
            console.log("data:")
            console.log(data)
            if (data)
                setAuthor(data);
            else
                setAuthor([]);
        });
    }, [slug]);


    return (
        <div className="card mb-5 ">

            <div className="row g-0">
                <div className="col-md-3">
                    <Card.Img
                        src={imageUrl}
                        className="img-fluid rounded-start"
                        alt={author.title} />
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
                                {postedDate.toLocaleString()}
                            </span>
                        </div>
                        <br />
                        <small className='text-muted'>
                            By:     
                            </small>
                            <span className='text-decoration-none fw-bold'>                 
                           
                            {author?.urlSlug}
                            </span> 
                            
                        <br />
                        {/* <small className='text-muted'>
                            Category:
                        </small>
                        <Link
                            className='text-decoration-none fw-bold'
                            to={`/category/${author?.category?.urlSlug}`}>
                            {author?.category?.name}
                        </Link> */}
                        {/* 
                        <div className='mb-3'>
                            <TagList tags={author.tags} />
                        </div>

                        <p className='fw-bold text-success'>
                            {author?.shortDescription}
                        </p>

                        <div
                            className='post-content'
                            dangerouslySetInnerHTML={{ __html: author.description }}>
                        </div> */}

                    </div>
                </div>
            </div>
        </div>
    );
}

export default PostsByAuthor;