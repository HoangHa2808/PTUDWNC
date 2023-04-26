import React, { useEffect, useState } from 'react';
import Table from 'react-bootstrap/Table';
import { Link, useParams } from 'react-router-dom';
import { getPostsFilter } from '../../../services/BlogRepository';
import Loading from '../../../components/Loading';
// import { isInteger } from '../../../utils/Utils';
import PostFilterPane from '../../../components/admin/PostFilterPane';
import { useSelector } from 'react-redux';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheck, faTrash, faXmark } from '@fortawesome/free-solid-svg-icons';

export default function Posts() {
    const [postsList, setPostsList] = useState([]),
        [isVisibleLoading, setIsVisibleLoading] = useState(true),
        postFilter = useSelector(state => state.postFilter);

    // const handleChange = (e) => {
    //     setPostsList(e.target.value)
    // }

    let { id } = useParams(),
        p = 1,
        ps = 100;

    useEffect(() => {
        document.title = 'Danh sách bài viết';
        getPostsFilter(postFilter.keyword,
            postFilter.authorId,
            postFilter.categoryId,
            postFilter.year,
            postFilter.month,
            ps, p).then(data => {
                if (data)
                    setPostsList(data.items);
                else
                    setPostsList([]);
                setIsVisibleLoading(false);
            });
    }, [
        postFilter.keyword,
        postFilter.authorId,
        postFilter.categoryId,
        postFilter.year,
        postFilter.month,
        p, ps
    ]);


    return (
        <>
            <h1>Danh sách bài viết {id}</h1>
            <PostFilterPane />
            {isVisibleLoading ? <Loading /> :
                <Table striped responsive bordered>
                    <thead>
                        <tr>
                            <th>Tiêu đề</th>
                            <th>Tác giả</th>
                            <th>Chủ đề</th>
                            <th>Xuất bản</th>
                            <th>Chỉnh sửa</th>

                        </tr>
                    </thead>
                    <tbody>
                        {postsList.length > 0 ? postsList.map((item, index) =>
                            <tr key={index}>
                                <td>
                                    <Link
                                        to={`/admin/posts/edit/${item.id}`}
                                        className='text-bold'>
                                        {item.title}
                                    </Link>
                                    <p className='text-muted'>{item.shortDescription}</p>
                                </td>
                                <td>{item.author.fullName}</td>
                                <td>{item.category.name}</td>
                                <td> <Link
                                    to={`/admin/posts/`}
                                    // className='text-bold'
                                    type="submit"
                                    className='icon ml-5' id='click' onClick={'myClick()'}
                                  >
                                    {/* class="btn btn-sm  "> */}
                                    {/* {item.published ? "btn-success" : "btn-danger"} */}
                                    {/* if (item.published) {
                                        <i class="fa fa-check"> Xuất bản</i>
                                    }else{
                                        <i class="fa fa-times"> Chưa xuất bản</i>
                                    } */}

                                    <span>
                                    <div >
                                        {item.published ?<FontAwesomeIcon icon={faCheck} /> :
                                            <FontAwesomeIcon icon={faXmark} />
                                        }
                                    <script>
                                        if (item.published ? "btn-success" : "btn-danger") {      
                                        function myClick() {
                                            document.getElementById("click").script.icon=<FontAwesomeIcon icon={faCheck} />
                                        } 
                                        }else{
                                            function myClick() {
                                                document.getElementById("click").script.icon=<FontAwesomeIcon icon={faXmark} />
                                            } 
                                        }

                                    </script>
                                    </div>
                                    </span>
                                </Link>
                                 
                                </td>
                                <td> <Link
                                    to={`/admin/posts`}
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