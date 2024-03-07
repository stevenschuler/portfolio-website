import React from "react"
// import { Link } from 'gatsby'
import { Helmet } from "react-helmet"
import Intro from "../Components/Intro/Intro"
import About from "../Components/About/About"
import Projects from "../Components/Projects/Projects"
import Contact from "../Components/Contact/Contact"
import "../Styles/global.css"

const IndexPage = () => {
  return (
    <main>
      <Helmet>
      <link href="https://fonts.googleapis.com/css2?family=Prompt&display=swap" rel="stylesheet"></link>
      <title>Steven Schuler</title> 
      <meta name="viewport" content="width=device-width, initial-scale=1"/>
      </Helmet>
    
      <div className="page-layout">
        <Intro role="developer" backgroundColor="orangered"/>
        <About backgroundColor="#e5e8e7"/>
        <Projects backgroundColor="#c9c9c9"/>
        <Contact backgroundColor="#e5e8e7"/>
      </div>
      
    </main>
  )
}

// export const Head = () => 

export default IndexPage;
