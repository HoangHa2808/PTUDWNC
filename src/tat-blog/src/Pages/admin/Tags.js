import React, { useEffect, useState } from 'react';
import Table from 'react-bootstrap/Table';
import { Link } from 'react-router-dom';
import { getTags} from '../../services/BlogRepository';
import Loading from '../../components/Loading';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPencil, faTrash } from '@fortawesome/free-solid-svg-icons';

export default function Tags() {
    const [tagsList, setTagsList] = useState([]),
        [isVisibleLoading, setIsVisibleLoading] = useState(true);


        useEffect(() => {
            document.title = 'Danh sách thẻ';
            getTags().then(data => {
                console.log("data:")
                console.log(data)
                    if (data)
                    setTagsList(data);
                    else
                    setTagsList([]);
                    setIsVisibleLoading(false);
                });
        }, []);

    return (
        <>
            <h1>Danh sách thẻ </h1>
            {isVisibleLoading ? <Loading /> :
                <Table striped responsive bordered>
                    <thead>
                        <tr>
                            <th>Tên</th>
                            <th>Slug</th>
                            <th>Chỉnh sửa</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tagsList.length > 0 ? tagsList.map((items, index) =>
                            <tr key={index}>
                                <td>
                                    <Link
                                        to={`/admin/tags/edit/${items.id}`}
                                        className='text-bold'>
                                        {items.name}
                                    </Link>
                                    <p className='text-muted'>{items.description}</p>
                                </td>
                                <td>{items.urlSlug}</td>
                                <td> <Link
                                    to={`/admin/tags/edit`}
                                    className='btn btn-tm mr-5'
                                    type="submit">
                                        <FontAwesomeIcon icon={faPencil}/> Cập nhật
                                </Link>
                                 <Link
                                    to={`/admin/tags/delete/${items.id}`}
                                    className='btn btn-tm mr-5'
                                    onClick="return confirm('Bạn có muốn xoá không?')">
                                    <FontAwesomeIcon icon={faTrash} /> Xoá
                                </Link></td>
                                </tr>
                        ) :
                            <tr>
                                <td colSpan={4}>
                                    <h4 className='text-danger text-center'>Không tìm thấy bài viết nào</h4>
                                </td>
                            </tr>}
                    </tbody>
                </Table>
            }
        </>
    );
}


