import { Link } from "react-router-dom";
import './index.css';

export default function NotFound() {
    return (
        < >
            <div className="text mt-5">
                <h1>Uh oh! Looks like you got lost.</h1>
                <p className="zoom-area">Go back to the homepage if you dare! </p>
                <section className="error-container">
                    <span className="four"><span className="screen-reader-text">4</span></span>
                    <span className="zero"><span className="screen-reader-text">0</span></span>
                    <span className="four"><span className="screen-reader-text">4</span></span>
                </section>
                <div className="link-container">
                    <Link target="_blank" to="http://localhost:3000/"
                        className="more-link">I dare!</Link>
                </div>
            </div>
        </ >
    );
}