import React, { useEffect, useState } from 'react';
import { isInteger, } from '../../utils/Utils';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { Link, useParams, Navigate } from 'react-router-dom';
import { getCategoryById, addOrUpdateCategory } from '../../services/BlogRepository';

export default function CategoryEdit() {
    const [validated, setValidated] = useState(false);
    const initialState = {
        id: 0,
        name: '',
        urlSlug: '',
        showOnMenu: false,
    },
        [category, setCategory] = useState(initialState);

    let { id } = useParams();
    id = id ?? 0;

    useEffect(() => {
        document.title = 'Thêm/cập nhật chủ đề';
        getCategoryById(id).then(data => {
            if (data)
            setCategory({
                    ...data,
                    // selectedTags: data.tags.map(tag => tag?.name).join('\r\n'),
                });
            else
                setCategory(initialState);
        });
    }, [])

    const handleSubmit = (e) => {
        e.preventDefault();
        if (e.currentTarget.checkValidity() === false) {
            e.stopPropagation();
            setValidated(true);
        } else {
            let form = new FormData(e.target);
            addOrUpdateCategory(form).then(data => {
                if (data)
                    alert('Đã lưu thành công!');
                else
                    alert('Đã xảy ra lỗi!');
            });
        }
    }

    if (id && !isInteger(id))
        return (
            <Navigate to={`/400?redirectTo=/admin/categories`} />)
    return (
        <>
            <Form
                method='post'
                encType='multipart/form-data'
                onSubmit={handleSubmit}
                noValidate validated={validated}
            >
                <Form.Control type='hidden' name='id' value={category.id} />  <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>
                        Chủ đề
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            type='text'
                            name='name'
                            title='name'
                            required
                            value={category.name || ''}
                            onChange={e => setCategory({
                                ...category,
                                name: e.target.value
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
                            value={category.urlSlug || ''}
                            onChange={e => setCategory({
                                ...category,
                                urlSlug: e.target.value
                            })}
                        />
                    </div>
                </div>
                <div className='row mb-3'>
                    <div className='col-sm-10 offset-sm-2'>
                        <div className='form-check'>
                            <input
                                className='form-check-input'
                                type='checkbox'
                                name='showOnMenu'
                                checked={category.showOnMenu}
                                title='ShowOnMenu'
                                onChange={e => setCategory({
                                    ...category,
                                    showOnMenu: e.target.checked
                                })}
                            />
                            <Form.Label className='form-check-label'>
                                Hiển thị
                            </Form.Label>
                        </div>
                    </div>
                </div>
                <div className='text-center'>
                    <Button variant='primary' type='submit'>
                        Lưu các thay đổi
                    </Button>
                    <Link to='/admin/categories' className='btn btn-danger ms-2'>  Hủy và quay lại
                    </Link>
                </div>
            </Form>
        </>
    );
}