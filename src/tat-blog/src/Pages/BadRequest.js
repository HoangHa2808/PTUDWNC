import { Link } from 'react-router-dom';
// import { useQuery } from '../utils/Utils';

export default function BadRequest() {
    // let query = useQuery(),
    //     redirectTo = query.get('redirectTo') ?? '/';

    return (
        <>
           <div className="text mt-5">
                <h1>Uh oh! The request hostname is invalid.</h1>
                <p className="zoom-area">Go back to the homepage if you dare! </p>
                <section className="error-container">
                    <span className="four"><span className="screen-reader-text">4</span></span>
                    <span className="zero"><span className="screen-reader-text">0</span></span>
                    <span className="zero"><span className="screen-reader-text">0</span></span>
                </section>
                <div className="link-container">
                    <Link target="_blank" to="http://localhost:3000/"
                        className="more-link">I dare!</Link>
                </div>
            </div>
        </>
    );
}

