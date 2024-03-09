import React from "react";
import "./Projects.css";
import SectionHeader from "../../Components/Global/SectionHeader"
import PropTypes from "prop-types";
import Project from "./Project";

const Projects = ({bgLight, bgDark}) => {
    const lightBackground = {
      backgroundColor: bgLight,
    };
    const darkBackground = {
      backgroundColor: bgDark,
    };

  return (
      <div>
        <div className="projects-section" style={darkBackground}>
          <SectionHeader headerText={"Personal Projects"} alignLeft={false} />

          <Project  
          id = {1}
          disclaimer={false}
          flip={true}/>
          <Project  
          id = {2}
          disclaimer={false}
          flip={true}/>
          </div>

        <div className="projects-section" style={lightBackground}>
        <SectionHeader headerText={"School Projects"} alignLeft={true} />
          
          <Project 
          id = {3}
          disclaimer={false}
          flip={false}/>
          <Project  
          id = {4}
          disclaimer={true}
          flip={false}/>
        </div>
      </div>
  );
};

Projects.propTypes = {
    bgLight: PropTypes.string,
    bgDark: PropTypes.string,
  };

export default Projects;