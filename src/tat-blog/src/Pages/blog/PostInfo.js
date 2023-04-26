import React, { useEffect, useState } from 'react'
import { Link, useParams } from "react-router-dom";
import TagList from "../../components/blog/TagList";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { isEmptyOrSpaces } from '../../utils/Utils'

// Lấy dữ liệu
import { getPost } from "../../services/BlogRepository";
import { Card } from 'react-bootstrap';



const PostInfo = () => {
    const [post, setPost] = useState([]);
    const { slug } = useParams();

    useEffect(() => {
        window.scrollTo(0, 0)
        getPost(slug).then(data => {
            console.log("data:")
            console.log(data)
            if (data)
                setPost(data);
            else
                setPost([]);
        });
    }, [slug]);

    let imageUrl = isEmptyOrSpaces(post.imageUrl)
        ? process.env.PUBLIC_URL + '/images/image_1.jpg'
        : `${post.imageUrl}`;

    let postedDate = new Date(post.postedDate);
    // if (post == null || post.length === 0) return (<div></div>)
    return (
        <div className="card mb-3 ">

            <div className="row g-0">
                <div className="col-md-3">
                    <Card.Img
                        src={imageUrl}
                        className="img-fluid rounded-start"
                        alt={post.title} />
                </div>
                <div className="col-md-9">
                    <div className="card-body">
                        <Link
                            className='card-title text-decoration-none'
                            to={`blog/post/${post.urlSlug}`}
                        >
                            {post.title}
                        </Link>
                        <div className="card-text">
                            {/* <small className="text-muted">
                                Category: {post.categoryName}
                            </small>
                            <br/>
                            <small className="text-muted">
                                Author: {post.authorName}
                            </small> */}
                            <small className='text-muted'>
                                Published on:
                            </small>
                            <span className='fw-bold text-primary'>
                                {postedDate.toLocaleString()}
                            </span>
                        </div>
                        <small className='text-muted'>
                            By:
                        </small>
                        <Link to={`blog/author/${post?.author?.slug}`}
                            className='text-decoration-none fw-bold'>
                            {post?.author?.fullName}
                        </Link>
                        <br />
                        <small className='text-muted'>
                            Category:
                        </small>
                        <Link
                            className='text-decoration-none fw-bold'
                            to={`blog/category/${post?.category?.slug}`}>
                            {post?.category?.name}
                        </Link>

                        <div className='mb-3'>
                            <TagList tags={post?.tags} />
                        </div>

                        <p className='fw-bold text-success'>
                            {post?.shortDescription}
                        </p>

                        <div
                            className='post-content'
                            dangerouslySetInnerHTML={{ __html: post?.description }}>
                        </div>

                        <div>
                            <div class="fb-share-button" data-href="https://www.facebook.com/photo?fbid=1624474121360621&set=a.121611258313589" data-layout="button_count" data-size="large">
                                <Link to="https://www.facebook.com/"
                                    class="btn btn-social btn-facebook link-in-popup">
                                    <FontAwesomeIcon icon="fa-brands fa-facebook" />Chia sẻ</Link>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );

};


export default PostInfo;