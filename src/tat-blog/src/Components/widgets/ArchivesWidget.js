import { useState, useEffect } from 'react';
import ListGroup from 'react-bootstrap/ListGroup';
import { Link } from 'react-router-dom';
import { getMonth } from '../../utils/Utils';
import { getArchives } from "../../services/Widgets";

const ArchivesWidget = () => {
    const [archivesList, setArchivesList] = useState([]);

    useEffect(() => {
        getArchives(12).then(data => {
            if (data)
            setArchivesList(data);
            else
            setArchivesList([]);
        });
    }, [])

    return (
        <div className='mb-4'>
            <h3 className='text-success mb-2'>
                Bài viết theo 12 tháng
            </h3>
            {archivesList.length > 0 &&
                <ListGroup>
                    {archivesList.map((item, index) => {
                        return (
                            <ListGroup.Item key={index}>
                                <Link
									to={`/blog/archive/${item.year}/${item.month}`}
								>
									{`${getMonth(item.month)} ${
										item.year
									} (${item.postCount})`}
								</Link>
                            </ListGroup.Item>
                        );
                    })}
                </ListGroup>
            }
        </div>
    );
}

export default ArchivesWidget; 