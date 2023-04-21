import React from "react";
import SearchForm from "./SearchForm";
import {CategoriesWidget, 
    FeaturtePostsWidget,
    RandomPostsWidget,
    ArchivesWidget,
    BestAuthorsWidgert,
    TagCloudWidget,
} from "../widgets";
// import FeaturtePostsWidget from "./widgets/FeaturtePostsWidget";
// import RandomPostsWidget from "./widgets/RandomPostsWidget";
// import ArchivesWidget from "./widgets/ArchivesWidget";
// import BestAuthorsWidgert from "./widgets/BestAuthorsWidgert";
// import TagCloudWidget from "./widgets/TagCloudWidget";


const Sidebar = () => {
    return(
        <div className="pt-4 ps-2">
            <SearchForm/>

            <CategoriesWidget />
                     
           <FeaturtePostsWidget/>

           <RandomPostsWidget/>

           <ArchivesWidget/>

           <BestAuthorsWidgert/>

           <TagCloudWidget/>
            <h1>
                Đăng ký nhận tin mới
            </h1>
           
        </div>
    )
}

export default Sidebar;