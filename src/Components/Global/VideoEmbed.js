import React from "react";
import PropTypes from "prop-types";

const VideoEmbed = ({link}) => {
  console.log(link.PropTypes);
  return (
    <div>
        <iframe width="560" height="315" 
        src={link} frameborder="0" 
        allowFullScreen
        title="Video Player">
        </iframe>
    </div>
  );
};

VideoEmbed.propTypes = {
  link: PropTypes.string.isRequired,
}

export default VideoEmbed;
