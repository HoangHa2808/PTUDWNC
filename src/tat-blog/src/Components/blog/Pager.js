import { Link } from "react-router-dom";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faArrowLeft, faArrowRight } from '@fortawesome/free-solid-svg-icons'
import Button from 'react-bootstrap/Button';

const Pager = ({ postquery, metadata }) => {

    let pageCount = metadata.pageCount,
        hasNextPage = metadata.hasNextPage,
        hasPreviousPage = metadata.hasPreviousPage,
        pageNumber = metadata.pageNumber,
        pageSize = metadata.pageSize,
        actionName = '', slug = '',
        keyword = postquery.keyword ?? '';

    if (pageCount > 1) {
        return (
            <div className='text-center my-4'>
                {hasPreviousPage
                    ? <Link
                        to={`/blog/${actionName}?slug=${slug}&k=${keyword}
                        &p=${pageNumber - 1}&ps=${pageSize}`}
                        className='btn btn-info'>
                        <FontAwesomeIcon icon={faArrowLeft} />
                        &nbsp;Trang trước
                    </Link>
                    : <Button variant='outline-secondary' disabled>
                        <FontAwesomeIcon icon={faArrowLeft} />
                        &nbsp;Trang trước
                    </Button>
                }
                {hasNextPage
                    ? <Link
                        to={`/blog/${actionName}?slug=${slug}&k=${keyword}
                        &p=${pageNumber + 1}&ps=${pageSize}`}
                        className='btn btn-info ms-1'>
                        Trang sau&nbsp;
                        <FontAwesomeIcon icon={faArrowRight} />
                    </Link>
                    : <Button variant='outline-secondary' className='ms-1' disabled>
                        Trang sau&nbsp;
                        <FontAwesomeIcon icon={faArrowRight} />
                    </Button>
                }
            </div>
        );
    }
    return (
        <Link>
        </Link>
    );
}

export default Pager;