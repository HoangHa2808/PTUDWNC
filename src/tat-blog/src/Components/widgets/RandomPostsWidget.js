import { useState, useEffect } from 'react';
import ListGroup from 'react-bootstrap/ListGroup';
import { Link } from 'react-router-dom';
import { getRandomPosts } from "../../services/Widgets";

const RandomPostsWidget = () => {
    const [randomList, setRandomPost] = useState([]);

    useEffect(() => {
        getRandomPosts(5).then(data => {
            if (data)
            setRandomPost(data);
            else
            setRandomPost([]);
        });
    }, []);


    return (
        <div className='mb-4'>
            <h3 className='text-success mb-2'>
                Bài viết ngẫu nhiên
            </h3>
            {randomList.length > 0 &&
                <ListGroup>
                    {randomList.map((item, index) => {
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

export default RandomPostsWidget; 