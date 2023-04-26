import React, { useEffect, useState } from 'react';
import { isInteger, decode } from '../../utils/Utils';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { Link, useParams, Navigate } from 'react-router-dom';
import { isEmptyOrSpaces } from '../../utils/Utils';
import { getAuthorById, addOrUpdateAuthor } from '../../services/BlogRepository';

export default function AuthorEdit() {
    const [validated, setValidated] = useState(false);
    const initialState = {
        id: 0,
        fullName: '',
        email:'',
        urlSlug: '',
        notes:'',
        imageUrl: '',
        joinedDate: '',
    },
        [author, setAuthor] = useState(initialState);

    let { id } = useParams();
    id = id ?? 0;

    useEffect(() => {
        document.title = 'Thêm/cập nhật tác giả';
        getAuthorById(id).then(data => {
            if (data)
                setAuthor({
                    ...data,
                    // selectedTags: data.tags.map(tag => tag?.name).join('\r\n'),
                });
            else
                setAuthor(initialState);
        });
    }, [])

    const handleSubmit = (e) => {
        e.preventDefault();
        if (e.currentTarget.checkValidity() === false) {
            e.stopPropagation();
            setValidated(true);
        } else {
            let form = new FormData(e.target);
            form.append('id', author.id);
            addOrUpdateAuthor(form).then(data => {
                console.log("data:")
                console.log(data)
                if (data)
                    alert('Đã lưu thành công!');
                else
                    alert('Đã xảy ra lỗi!');
            });
        }
    }

    if (id && !isInteger(id))
        return (
            <Navigate to={`/400?redirectTo=/admin/authors`} />)
    return (
        <>
            <Form
                method='post'
                encType='multipart/form-data'
                onSubmit={handleSubmit}
                noValidate 
                validated={validated}
            >
                
                <Form.Control type='hidden' name='id' value={author.id} />  <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>
                        Tên tác giả
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            type='text'
                            name='fullName'
                            title='FullName'
                            required
                            value={author.fullName || ''}
                            onChange={e => setAuthor({
                                ...author,
                                fullName: e.target.value
                            })}
                        />
                    </div>
                </div>
                <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>
                        Email
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            type='text'
                            name='email'
                            title='Email'
                            value={author.email || ''}
                            onChange={e => setAuthor({
                                ...author,
                                email: e.target.value
                            })}
                        />
                    </div>
                </div>
                <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>
                        Slug
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            type='text'
                            name='urlSlug'
                            title='Url slug'
                            value={author.urlSlug || ''}
                            onChange={e => setAuthor({
                                ...author,
                                urlSlug: e.target.value
                            })}
                        />
                    </div>
                </div>
             
                <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>
                        Ngày viết bài
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            as='textarea'
                            type='text'
                            required
                            name='shortDescription'
                            title='Short description'
                            value={decode(author.joinedDate || '')} onChange={e => setAuthor({
                                ...author,
                                joinedDate: e.target.value
                            })}
                        />
                    </div>
                </div>

                <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>
                        Slug
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            type='text'
                            name='notes'
                            title='Notes'
                            value={author.notes || ''}
                            onChange={e => setAuthor({
                                ...author,
                                notes: e.target.value
                            })}
                        />
                    </div>
                </div>

                {!isEmptyOrSpaces(author.imageUrl) && <div className='row mb-3'>  <Form.Label className='col-sm-2 col-form-label'>
                    Hình hiện tại
                </Form.Label>
                    <div className='col-sm-10'>
                        <img src={author.imageUrl} alt={author.title} />  </div>
                </div>
                }
                <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>  Chọn hình ảnh
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            type='file'
                            name='imageFile'
                            accept='image/*'
                            title='Image file'
                            onChange={e => setAuthor({
                                ...author,
                                imageFile: e.target.files[0]
                            })}
                        />
                    </div>
                </div>
                <div className='text-center'>
                    <Button variant='primary' type='submit'>
                        Lưu các thay đổi
                    </Button>
                    <Link to='/admin/posts' className='btn btn-danger ms-2'>  Hủy và quay lại
                    </Link>
                </div>
            </Form>
        </>
    );
}

