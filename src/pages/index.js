import * as React from "react"
import { Link } from 'gatsby'
import Layout from './Layout'

const IndexPage = () => {
  return (
    <main>
      <Layout pageTitle="Main Page">
        <p> This is my home page!</p>
      </Layout>
    </main>
  )
}

export const Head = () => <title>Steven</title>

export default IndexPage
