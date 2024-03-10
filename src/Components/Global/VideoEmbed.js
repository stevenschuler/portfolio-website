import React, { useEffect } from "react";
import PropTypes from "prop-types";
import LazyLoad from "vanilla-lazyload";

const VideoEmbed = ({link}) => {
  useEffect(() => {
    const lazyLoadInstance = new LazyLoad({
      elements_selector: '.lazy',
    });
    return () => lazyLoadInstance.destroy();
  }, []);

  return (
    <div>
        <iframe 
        className="lazy"
        width="560" height="315" 
        data-src={link} frameBorder="0" 
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
