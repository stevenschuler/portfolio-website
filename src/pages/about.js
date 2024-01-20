import * as React from 'react'
import {Link} from "gatsby"
import Layout from './Layout'

const AboutPage = () => {
    return (
        <main> 
        <Layout pageTitle="About Me">
            <p>This is my about page</p>
        </Layout>
        </main>
    )
}

export const Head = () => <title>About Me</title>

export default AboutPage