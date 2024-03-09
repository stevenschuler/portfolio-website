import React from "react";
import PropTypes from "prop-types";
import VideoEmbed from "../Global/VideoEmbed.js"
import projectData from "../Projects/Projects.json";

const Project = ({id, disclaimer, flip}) => {
  
    const flippedStyle = {
        "flexDirection": flip ? 'row-reverse' : 'row'
    };
    const textMargin = {
        "marginRight": flip ? "0px" : "20px",
        "marginLeft": flip ? "20px" : "0px"
    };
    const smallText = {
      "fontSize": "small"
    };

    const project = projectData.projects.find((p) => p.id === id);
    if (!project){
      return <div>Project not found!</div>
    }

  return (
    <div className="project-layout" style={flippedStyle}>
        <div className="project-description" style={textMargin}>
            <b>[ {project.name} ]</b>

            

            <p>{project.description}</p>
            <b>Skills Used</b>
            <ul>
            {project.skills.map((skill, index) => (
              <li key={index}>{skill}</li>
            ))}
            </ul>
            <a href={project.github} target="_blank" rel="noopener noreferrer"> Relevant Code (GitHub)</a>
        </div>    
        {disclaimer ? (
              <p style={smallText}>{projectData.disclaimer}</p>
            ) :(
              <VideoEmbed link={project.youtube}/>
            )}
    </div>
  );
};


export default Project;