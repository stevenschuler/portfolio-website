import React from "react";
import PropTypes from "prop-types";
import VideoEmbed from "../Global/VideoEmbed.js"

const Project = ({data, link, github, flip}) => {
    const flippedStyle = {
        "flexDirection": flip ? 'row-reverse' : 'row'
    };
    const textMargin = {
        "marginRight": flip ? "0px" : "20px",
        "marginLeft": flip ? "20px" : "0px"
    };

  return (
    <div className="project-layout" style={flippedStyle}>
        <div className="project-description" style={textMargin}>
            <p>{data}</p>
            <a href={github} target="_blank" rel="noopener noreferrer"> Relevant Code (GitHub)</a>
        </div>    
    <VideoEmbed link={link}/>
    </div>
  );
};

Project.propTypes = {
    data: PropTypes.string.isRequired,
    link: PropTypes.string,
    github: PropTypes.string,
    flip: PropTypes.bool.isRequired,
  };

export default Project;