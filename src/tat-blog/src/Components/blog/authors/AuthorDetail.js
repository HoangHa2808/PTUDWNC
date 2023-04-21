import React from 'react'

const AuthorDetail = ({authorSlug}) => {

  return (
    <div className='bg-light p-3'>
        <div className='row'>
            <div className='col-2'>
                <img
                    className="img-fluid"
                    src="/image_3.png" 
                    alt="Author" />
            </div>
            <div className='col-10'>
                <h3 className='text-success'>
                    Author Name {authorSlug}
                </h3>
                <p>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit. 
                    Cumque, quia provident! Nulla possimus, cupiditate mollitia 
                    at quasi illum doloribus quibusdam exercitationem amet enim 
                    veniam sunt assumenda labore, hic quidem ut. Sunt id, ex 
                    obcaecati suscipit illum accusamus! Nisi, ipsa saepe. Saepe 
                    hic laborum atque obcaecati, dolores ullam quis aut et!
                </p>
            </div>
        </div>
    </div>
  )
}

export default AuthorDetail;