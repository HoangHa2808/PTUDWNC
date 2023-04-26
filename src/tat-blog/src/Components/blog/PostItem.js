import TagList from "./TagList";
import { Card } from "react-bootstrap";
import { Link } from "react-router-dom";
import { isEmptyOrSpaces } from '../../utils/Utils'
import { useEffect } from "react";

const PostList = ({ postItem }) => {
    let imageUrl = isEmptyOrSpaces(postItem.imageUrl)
        ? process.env.PUBLIC_URL + '/images/image_1.jpg'
        : `${postItem.imageUrl}`;

    let postedDate = new Date(postItem.postedDate);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    return (
        <article className='blog-entry mb-4'>
            <Card>
                <div className='row g-0'>
                    <div className='col-md-4'>
                        <Card.Img variant='top' src={imageUrl} alt={postItem.title} />
                    </div>
                    <div className='col-md-8'>
                        <Card.Body>
                            <Card.Title>
                                <Link to={`/blog/post/${postedDate.getFullYear()}/${postedDate.getMonth()}/${postedDate.getDay()}/${postItem.urlSlug}}`}>
                                </Link>{postItem.title}
                            </Card.Title>
                            <Card.Text>
                                <small className='text-muted'>Tác giả:</small>
                                <Link to={`blog/author/${postItem.author.urlSlug}`}>
                                    <span className='text-primary m-1'>
                                        {postItem.author.fullName}
                                    </span>
                                </Link>
                                <small className='text-muted'>Chủ đề:</small>
                                <Link to={`blog/category/${postItem.category.urlSlug}`}>
                                    <span className='text-primary m-1'>
                                        {postItem.category.name}
                                    </span>
                                </Link>
                            </Card.Text>

                            <Card.Text>
                                {postItem.shortDescription}
                            </Card.Text>
                            <div className='tag-list'>
                                <TagList tagList={postItem.tags} />
                            </div>
                            <div className='text-end'>
                                <Link to={`/blog/post/${postedDate.getFullYear()}/${postedDate.getMonth()}/${postedDate.getDay()}/${postItem.urlSlug}`}
                                    className='btn btn-primary'
                                    title={postItem.title}>
                                    Xem chi tiết
                                </Link>
                            </div>
                        </Card.Body>
                    </div>
                </div>
            </Card>
        </article >
    );
};

export default PostList;