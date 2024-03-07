import React from "react";
import "./Intro.css";
import PropTypes from "prop-types";
import { StaticImage } from "gatsby-plugin-image"

const Intro = ({role, backgroundColor}) => {
  const bgc = {
    backgroundColor,
  };

  return (
    <div className="intro-section" style={bgc}>
      <div className="intro-text-container">
      <h1 className="intro-pre-text">{"Hi, my name is"}</h1>
      <h1 className="intro-name">{"Steven Schuler"}</h1>
      <h1 className="intro-post-text">{`aspiring ${role}`}</h1>
      </div>
      <div className="intro-face-container">
      <StaticImage className="intro-face-pic" src="../../images/steven_face.png" alt={`Steven's face`} />
      </div>
    </div>
  );
};

Intro.propTypes = {
  role: PropTypes.string.isRequired,
  backgroundColor: PropTypes.string,
};

export default Intro;