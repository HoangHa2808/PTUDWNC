import React, { useEffect, useState } from 'react';
import { isInteger, } from '../../utils/Utils';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { Link, useParams, Navigate } from 'react-router-dom';
import { getTagById, addOrUpdateTag } from '../../services/BlogRepository';

export default function TagEdit() {
    const [validated, setValidated] = useState(false);
    const initialState = {
        id: 0,
        name: '',
        urlSlug: '',
    },
        [tag, setTag] = useState(initialState);

    let { id } = useParams();
    id = id ?? 0;

    useEffect(() => {
        document.title = 'Thêm/cập nhật thẻ';
        getTagById(id).then(data => {
            if (data)
            setTag({
                    ...data,
                    // selectedTags: data.tags.map(tag => tag?.name).join('\r\n'),
                });
            else
                setTag(initialState);
        });
    }, [])

    const handleSubmit = (e) => {
        e.preventDefault();
        if (e.currentTarget.checkValidity() === false) {
            e.stopPropagation();
            setValidated(true);
        } else {
            let form = new FormData(e.target);
            addOrUpdateTag(form).then(data => {
                if (data)
                    alert('Đã lưu thành công!');
                else
                    alert('Đã xảy ra lỗi!');
            });
        }
    }

    if (id && !isInteger(id))
        return (
            <Navigate to={`/400?redirectTo=/admin/tags`} />)
    return (
        <>
            <Form
                method='post'
                encType='multipart/form-data'
                onSubmit={handleSubmit}
                noValidate validated={validated}
            >
                <Form.Control type='hidden' name='id' value={tag.id} />  <div className='row mb-3'>
                    <Form.Label className='col-sm-2 col-form-label'>
                        Tên thẻ
                    </Form.Label>
                    <div className='col-sm-10'>
                        <Form.Control
                            type='text'
                            name='name'
                            title='name'
                            required
                            value={tag.name || ''}
                            onChange={e => setTag({
                                ...tag,
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
                            value={tag.urlSlug || ''}
                            onChange={e => setTag({
                                ...tag,
                                urlSlug: e.target.value
                            })}
                        />
                    </div>
                </div>
                
                <div className='text-center'>
                    <Button variant='primary' type='submit'>
                        Lưu các thay đổi
                    </Button>
                    <Link to='/admin/tags' className='btn btn-danger ms-2'>  Hủy và quay lại
                    </Link>
                </div>
            </Form>
        </>
    );
}