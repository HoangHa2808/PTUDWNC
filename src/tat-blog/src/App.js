import logo from './logo.svg';
import {
  BrowserRouter as Router, Routes, Route,
} from "react-router-dom";
import './App.css';
// import "assets/plugins/nucleo/css/nucleo.css";
// import "@fortawesome/fontawesome-free/css/all.min.css";
// import "assets/scss/argon-dashboard-react.scss";

// import Navbar from './components/blog/Navbar';
// import Sidebar from './components/blog/Sidebar;
import PostItem from './components/blog/PostItem';
import Footer from './components/blog/Footer';
// import Index from './pages/Index';
// import Layout from './pages/Layout';
// import About from './pages/blog/About';
// import Contact from './pages/Contact';
// import RSS from './pages/Rss';
// import PostInfo from './pages/blog/PostInfo';
import {
  BlogLayout,
  BlogHome,
  Contact,
  About,
  Rss,
  PostInfo,
  PostsByAuthor,
  PostsByCategory,
  PostsByTag,
  PostsByTime,
  NotFound,
  BadRequest,
  AdminLayout,
  AdminHome,
  Authors,
  AuthorEdit,
  Categories,
  Posts,
  Edit,
  Tags,
  Comments,
} from "./pages";

import TagEdit from './pages/admin/TagEdit';
import CategoryEdit from './pages/admin/CategoryEdit';

function App() {
  return (
      <Router>
        {/* <Navbar /> */}
        {/* <div className='container-fluid'>
          <div className='row'>
            <div className='col-9'> */}
              <Routes>
                <Route path='/' element={<BlogLayout />}>
                  <Route path='/' element={<BlogHome />} />
                  <Route path='blog' element={<BlogHome />} />
                  <Route path='blog/post/:year/:month/:day/:slug' element={<PostInfo />} />
                  <Route path='blog/Contact' element={<Contact />} />
                  <Route path='blog/About' element={<About />} />
                  <Route path='blog/RSS' element={<Rss />} />
                  <Route path='blog/author/:slug' element={<PostsByAuthor />} />
                  <Route path='blog/category/:slug' element={<PostsByCategory />} />
                  <Route path='blog/tag/:slug' element={<PostsByTag />} />
                  <Route path='blog/archive/:year/:month' element={<PostsByTime />} />
                  <Route path='blog/post/:slug' element={<PostInfo />} />
                </Route>
                
                <Route path='/admin' element={<AdminLayout />} >
                  <Route path='/admin' element={<AdminHome />} />
                  <Route path='/admin/authors' element={<Authors />} />
                  <Route path='/admin/authors/edit' element={<AuthorEdit />} />
                  <Route path='/admin/authors/edit/:id' element={<AuthorEdit />} />
                  <Route path='/admin/categories' element={<Categories />} />
                  <Route path='/admin/categories/edit' element={<CategoryEdit />} />
                  <Route path='/admin/categories/edit/:id' element={<CategoryEdit />} />
                  <Route path='/admin/comments' element={<Comments />} />
                  <Route path='/admin/posts' element={<Posts />} />
                  <Route path='/admin/posts/edit' element={<Edit />} /> 
                  <Route path='/admin/posts/edit/:id' element={<Edit />} />
                  <Route path='/admin/tags' element={<Tags />} />
                  <Route path='/admin/tags/edit' element={<TagEdit />} />
                  <Route path='/admin/tags/edit/:id' element={<TagEdit />} />
                </Route>

              <Route path='*' element={<NotFound />}/>
              <Route path='/400' element={<BadRequest />}/>
              </Routes>

            {/* </div>
            <div className='col-3 border-start'>
              <Sidebar />
            </div>
          </div>
        </div> */}
        
      </Router>
  );
}

export default App;
