import Spinner from 'react-bootstrap/Spinner';
import Button from 'react-bootstrap/Button';

export default function Loading(){
    return (
        <div className='text-center'>
            <Button variant='outline-success' 
            disabled style={{ border: 'none' }}>  <Spinner
                as='span'
                animation='grow'
                size='sm'
                role='status'
                aria-hidden='true'
            />
                &nbsp;Đang tải...
            </Button>
        </div>
    );
}
