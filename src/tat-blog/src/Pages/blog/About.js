import React, { useEffect } from "react";

const About = ({ aboutItem }) => {

    useEffect(() => {
        document.title = "Giới thiệu";

    }, []);

    return (
        <div>
            <div class="row align-items-center justify-content-center text-center pt-5">
                <div class="col-lg-6">
                    <h1 class="heading text-black mb-5" data-aos="fade-up">Giới thiệu</h1>
                </div>
            </div>
            <div class="section">
                <div class="container">
                    <p>
                        You are responsible for creating the content for this page
                    </p>

                    {/* <p className="text-center">
           <img className='img-fluid' src="/images/image.jpg" alt="TAT BLOG" />
         </p> */}

                    <p>
                        Lorem ipsum dolor sit, amet consectetur adipisicing elit. Itaque, facilis! Similique error autem facilis.
                        Iusto nisi reprehenderit consequatur pariatur accusamus atque, autem suscipit! Exercitationem culpa illo enim,
                        hic repudiandae soluta ducimus, architecto quod commodi ut voluptatum consequuntur iste, quidem maiores nihil
                        praesentium? Rerum voluptatibus eum omnis esse sit inventore numquam.
                    </p>

                    <p>
                        Lorem ipsum dolor sit, amet consectetur adipisicing elit. Itaque, facilis! Similique error autem facilis.
                        Iusto nisi reprehenderit consequatur pariatur accusamus atque, autem suscipit! Exercitationem culpa illo enim,
                        hic repudiandae soluta ducimus, architecto quod commodi ut voluptatum consequuntur iste, quidem maiores nihil
                        praesentium? Rerum voluptatibus eum omnis esse sit inventore numquam.
                    </p>

                    <p>
                        Lorem ipsum dolor sit, amet consectetur adipisicing elit. Itaque, facilis! Similique error autem facilis.
                        Iusto nisi reprehenderit consequatur pariatur accusamus atque, autem suscipit! Exercitationem culpa illo enim,
                        hic repudiandae soluta ducimus, architecto quod commodi ut voluptatum consequuntur iste, quidem maiores nihil
                        praesentium? Rerum voluptatibus eum omnis esse sit inventore numquam.
                    </p>
                </div>
            </div>
        </div>
    );
}

export default About;