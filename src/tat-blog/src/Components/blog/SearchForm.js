import { useState } from 'react';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSearch } from '@fortawesome/free-solid-svg-icons'

const SearchForm = () => {
    const [keyword, setKeyword] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        window.location = `/blog?k=${keyword}`;
    };

    return (
        <div className='mb-4'>
            <Form method='get' onSubmit={handleSubmit}>
                <Form.Group className='input-group mb-3'>
                    <Form.Control
                        type='text'
                        name='k'
                        value={keyword}
                        onChange={(e) => setKeyword(e.target.value)}
                        aria-label='Enter keyword'
                        aria-describedby='btnSearchPost'
                        placeholders=' Enter keyword' />
                    <Button
                        id='btnSearchPost'
                        variants='outline-secondary'
                        type='submit'>
                        <FontAwesomeIcon icon={faSearch} />
                    </Button>
                </Form.Group>
            </Form>
        </div>
    );
}

export default SearchForm;