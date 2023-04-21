import { useState, useEffect } from 'react';
import ListGroup from 'react-bootstrap/ListGroup';
import { Link } from 'react-router-dom';
import { getFeaturedPosts } from "../../services/Widgets";

const FeaturtePostsWidget = () => {
    const [featurteList, setFeaturtePost] = useState([]);

    useEffect(() => {
        getFeaturedPosts(3).then(data => {
            if (data)
            setFeaturtePost(data);
            else
            setFeaturtePost([]);
        });
    }, []);


    return (
        <div className='mb-4'>
            <h3 className='text-success mb-2'>
                Bài viết nổi bật
            </h3>
            {featurteList.length > 0 &&
                <ListGroup>
                    {featurteList.map((item, index) => {
                        const postedDate = new Date(index.postedDate);
                        return (
                            <ListGroup.Item key={index}>
                                <Link to={`/blog/post/${item.urlSlug}`}>
									{item.title}
                                    <span>&nbsp;({item.postCount})</span>
								</Link>
                            </ListGroup.Item>
                        );
                    })}
                </ListGroup>
            }
        </div>
    );
}

export default FeaturtePostsWidget; 