import React from 'react'
import { Link } from 'react-router-dom'

const TagItem = ({slug, tagName, postCount}) => {
  return (
    <Link 
        className='btn btn-sm btn-outline-secondary me-2 mb-2'
        to={`/tag/${slug}`}
        title="Click to view posts containing this tag"
    >
        {tagName} {postCount > 0 && `(${postCount})`}
    </Link>
  )
}

export default TagItem