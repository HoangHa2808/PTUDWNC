import React, { useEffect, useState } from 'react';
import Table from 'react-bootstrap/Table';
import { Link, } from 'react-router-dom';
import { deleteComment, getComments } from '../../services/BlogRepository';
import Loading from '../../components/Loading';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheck, faTrash, faXmark } from '@fortawesome/free-solid-svg-icons';
import { Form } from 'react-bootstrap';

export default function Comments() {
    const [tagsList, setTagsList] = useState([]),
        [isVisibleLoading, setIsVisibleLoading] = useState(true),
        [validated, setValidated] = useState(false);


    useEffect(() => {
        document.title = 'Danh sách bình luận';

        getComments().then(data => {
            console.log("data:")
            console.log(data)
            if (data)
                setTagsList(data);
            else
                setTagsList([]);
            setIsVisibleLoading(false);
        });
    }, []);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (e.currentTarget.checkValidity() === false) {
            e.stopPropagation();
            setValidated(true);
        } else {
            let form = new FormData(e.target);
            deleteComment(form).then(data => {
                if (data)
                    alert('Đã lưu thành công!');
                else
                    alert('Đã xảy ra lỗi!');
            });
        }
    }

    return (
        <>
            <h1>Danh sách bình luận </h1>
            {isVisibleLoading ? <Loading /> :
                <Table striped responsive bordered>
                    <thead>
                        <tr>
                            <th>Tên</th>
                            <th>Nội dung</th>
                            <th>Ngày bình luận</th>
                            <th>Trạng thái</th>
                            <th>Chỉnh sửa</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tagsList.length > 0 ? tagsList.map((items, index) =>
                            <tr key={index}>
                                <td>
                                    {items.name}

                                    {/* <p className='text-muted'>{items.description}</p> */}
                                </td>
                                <td>{items.description}</td>
                                <td>{items.postedDate}</td>
                                <td> <Link
                                    to={`/admin/comments`}
                                    className='text-bold'
                                    type="submit">
                                    <div className='icon ml-5'>
                                        {items.showOnMenu ? <FontAwesomeIcon icon={faCheck} /> :
                                            <FontAwesomeIcon icon={faXmark} />
                                        }
                                    </div>
                                </Link>
                                </td>
                                <td>
                                 <Link   
                                            to={`/admin/comments`}                                
                                            className='btn btn-tm mr-5'
                                            onClick={() => alert("Bạn có muốn xoá không?")}>
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


