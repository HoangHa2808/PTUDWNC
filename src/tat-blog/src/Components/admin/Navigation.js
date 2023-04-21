import React from 'react';
import { Navbar as Nb, Nav } from 'react-bootstrap';
import {
    Link
} from 'react-router-dom';

export default function Navigation() {
    return (
        <Nb collapseOnSelect expand='sm' bg='white' variant='light' 
        className='border-bottom shadow'>
            <div className='container-fluid'>
                <Nb.Brand href='/admin'>Tips & Tricks</Nb.Brand>
                <Nb.Toggle aria-controls='responsive-navbar-nav' />
                <Nb.Collapse id='responsive-navbar-nav' className='d-sm-inline-flex  justify-content-between'>
                    <Nav className='mr-auto flex-grow-1'>
                        <Nav.Item>
                            <Link to='/admin/categories' className='nav-link text-dark'>  
                            Chủ đề
                            </Link>
                        </Nav.Item>
                        <Nav.Item>
                            <Link to='/admin/authors' className='nav-link text-dark'>  
                            Tác giả
                            </Link>
                        </Nav.Item>
                        <Nav.Item>
                            <Link to='/admin/tags' className='nav-link text-dark'>  
                            Thẻ
                            </Link>
                        </Nav.Item>
                        <Nav.Item>
                            <Link to='/admin/posts' className='nav-link text-dark'> 
                            Bài viết
                            </Link>
                        </Nav.Item>
                        <Nav.Item>
                            <Link to='/admin/comments' className='nav-link text-dark'> 
                            Bình luận
                            </Link>
                        </Nav.Item>
                    </Nav>
                </Nb.Collapse>
            </div>
        </Nb>
    );
}
  
