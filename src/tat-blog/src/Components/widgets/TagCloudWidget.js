import { useState, useEffect } from 'react';
import ListGroup from 'react-bootstrap/ListGroup';
import { Link } from 'react-router-dom';
import { getTagCloud } from "../../Services/Widgets";

const TagCloudWidget = () => {
    const [tagList, setTagList] = useState([]);

    useEffect(() => {
        getTagCloud().then(data => {
            if (data)
            setTagList(data);
            else
            setTagList([]);
        });
    }, [])


    return (
        <div className='mb-4'>
            <h3 className='text-success mb-2'>
                Danh sách các thẻ
            </h3>
            {tagList.length > 0 &&
                <ListGroup>
                    {tagList.map((item, index) => {
                        return (
                            <ListGroup.Item key={index}>
                                <Link to={`/blog/tag/${item.urlSlug}`}
                                    title={item.decripsion}
                                    key={index}> 
                                    {item.name}
                                </Link>
                            </ListGroup.Item>
                        );
                    })}
                </ListGroup>
            }
        </div>
    );
}

export default TagCloudWidget; 