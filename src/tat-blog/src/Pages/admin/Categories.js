import React, { useEffect, useState } from 'react';
import Table from 'react-bootstrap/Table';
import { Link } from 'react-router-dom';
import { getCategories} from '../../services/BlogRepository';
import Loading from '../../components/Loading';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheck, faPencil, faTrash, faXmark } from '@fortawesome/free-solid-svg-icons';

export default function Categorties() {
    const [categoriesList, setCategoriesList] = useState([]),
        [isVisibleLoading, setIsVisibleLoading] = useState(true);


        useEffect(() => {
            document.title = 'Danh sách chủ đề';
         
            getCategories().then(data => {
                console.log("data:")
                console.log(data)
                    if (data)
                    setCategoriesList(data);
                    else
                    setCategoriesList([]);
                    setIsVisibleLoading(false);
                });
        }, []);

    return (
        <>
            <h1>Danh sách chủ đề </h1>
            {isVisibleLoading ? <Loading /> :
                <Table striped responsive bordered>
                    <thead>
                        <tr>
                            <th>Chủ đề</th>
                            <th>Slug</th>
                            <th>Show On Menu</th>
                            <th>Chỉnh sửa</th>
                        </tr>
                    </thead>
                    <tbody>
                        {categoriesList.length > 0 ? categoriesList.map((items, index) =>
                            <tr key={index}>
                                <td>
                                    <Link
                                        to={`/admin/categories/edit/${items.id}`}
                                        className='text-bold'>
                                        {items.name}
                                    </Link>
                                    <p className='text-muted'>{items.description}</p>
                                </td>
                                <td>{items.urlSlug}</td>
                                <td> <Link
                                    to={`/admin/categories/toggle/${items.id}`}
                                    className='text-bold'
                                    type="submit">
                                    <div className='icon ml-5'>
                                        {items.showOnMenu ? <FontAwesomeIcon icon={faCheck}/>: 
                                        <FontAwesomeIcon icon={faXmark}/> 
                                        }
                                    </div>
                                </Link>
                                </td>
                                <td>
                                <Link
                                    to={`/admin/categories/edit`}
                                    className='btn btn-tm mr-5'
                                    type="submit">
                                        <FontAwesomeIcon icon={faPencil}/> Cập nhật
                                </Link>
                                     <Link
                                    to={`/admin/categories/delete/${items.id}`}
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


