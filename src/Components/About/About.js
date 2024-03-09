import React from "react";
import "./About.css";
import aboutData from "../About/About.json";
import SectionHeader from "../../Components/Global/SectionHeader"
import PropTypes from "prop-types";




const About = ({backgroundColor}) => {
  const bgc = {
    backgroundColor,
  };
  return (
    <div className="about-section" style={bgc}>
        <SectionHeader headerText={"About Me"} alignLeft={true} />
        <p>{aboutData.p1}</p>
        <p>{aboutData.p2}</p>
        <p>{aboutData.p3}</p>
        <p>{aboutData.p4}</p>
        <p>{aboutData.p5}</p>
    </div>
  );
};

About.propTypes = {
  backgroundColor: PropTypes.string,
};

export default About;