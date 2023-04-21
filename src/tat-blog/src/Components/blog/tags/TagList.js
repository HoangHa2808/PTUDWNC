import React from 'react'
import TagItem from './TagItem';

const TagList = ({tags}) => {
  return (
    <div>
        {tags.map((item) => (
            <TagItem 
                key={item.id}
                slug={item.urlSlug}
                tagName={item.name}
                postCount={item.postCount} />
        ))}
    </div>
  )
}

export default TagList;