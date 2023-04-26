import React, { useEffect, useState } from 'react';
import Table from 'react-bootstrap/Table';
import { Link } from 'react-router-dom';
import { getAuthors} from '../../services/BlogRepository';
import Loading from '../../components/Loading';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPencil, faTrash } from '@fortawesome/free-solid-svg-icons';

export default function Authors() {
    const [authorsList, setAuthorsList] = useState([]),
        [isVisibleLoading, setIsVisibleLoading] = useState(true);


        useEffect(() => {
            document.title = 'Danh sách tác giả';
         
            getAuthors().then(data => {
                console.log("data:")
                console.log(data)
                    if (data)
                    setAuthorsList(data);
                    else
                    setAuthorsList([]);
                    setIsVisibleLoading(false);
                });
        }, []);

    return (
        <>
            <h1>Danh sách tác giả </h1>
            {isVisibleLoading ? <Loading /> :
                <Table striped responsive bordered>
                    <thead>
                        <tr>
                            <th>Tên tác giả</th>
                            <th>Slug</th>
                            <th>Số bài viết</th>
                            <th>Ngày viết bài</th>
                            <th>Hình ảnh</th>
                            <th>Chỉnh sửa</th>
                        </tr>
                    </thead>
                    <tbody>
                        {authorsList.length > 0 ? authorsList.map((items, index) =>
                            <tr key={index}>
                                <td>
                                    <Link
                                        to={`/admin/authors/edit/${items.id}`}
                                        className='text-bold'>
                                        {items.fullName}
                                    </Link>
                                    <p className='text-muted'>{items.email}</p>
                                </td>
                                <td>{items.urlSlug}</td>
                                <td>{items.postCount}</td>
                                <td>{items.joinedDate}</td> 
                                <td>{items.imageUrl}</td> 
                                <td>
                                <Link
                                    to={`/admin/authors/edit`}
                                    className='btn btn-tm mr-5'
                                    type="submit">
                                        <FontAwesomeIcon icon={faPencil}/> Cập nhật
                                </Link>
                                     <Link
                                    to={`/admin/authors/delete/${items.id}`}
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

