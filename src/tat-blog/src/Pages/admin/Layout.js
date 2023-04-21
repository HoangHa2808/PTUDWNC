import { Outlet } from 'react-router-dom';
import Navbar from '../../components/admin/Navigation';
// import Footer from '../../components/Footer';

export default function Layout() {
    return (
        <>
            <Navbar />
            <div className='container-fluid py-3'>
                <Outlet />
            </div>
        </>
    );
};
