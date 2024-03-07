import React from "react";
import "./Projects.css";
import projectData from "../Projects/Projects.json";
import SectionHeader from "../../Components/Global/SectionHeader"
import PropTypes from "prop-types";
import Project from "./Project";

const Projects = ({backgroundColor}) => {
    const bgc = {
        backgroundColor
    }
    const dummyGit = "github.com/asdfgsdfgdf"
  return (
    <div className="projects-section" style={bgc}>

        <SectionHeader headerText={"Personal Projects"} alignLeft={false} />
        <Project  
        data={projectData["personal-p1"]} 
        link="https://www.youtube.com/embed/rYR4jLY1Bsw?si=4IB1chwsZPjJi9oV"
        github={dummyGit}
        flip={true}/>
        <Project  
        data={projectData["personal-p2"]} 
        link="https://www.youtube.com/embed/fqOuJBc1fkQ?si=BOpYCSSF1UMjxUGR"
        github={dummyGit}
        flip={true}/>
        
        <SectionHeader headerText={"School Projects"} alignLeft={true} />
        <Project 
        data={projectData["school-p1"]} 
        link="https://www.youtube.com/embed/wW3XsJxuosE?si=B5foyGWU21UtThA8"
        github={dummyGit}
        flip={false}/>
        <Project  
        data={projectData["school-p2"]} 
        link="https://www.youtube.com/embed/rYR4jLY1Bsw?si=4IB1chwsZPjJi9oV"
        github={dummyGit}
        flip={false}/>

    </div>
  );
};

Projects.propTypes = {
    backgroundColor: PropTypes.string,
  };

export default Projects;