import * as React from "react"
import { Link } from 'gatsby'
import Layout from './Layout'
import { StaticImage } from "gatsby-plugin-image"

const IndexPage = () => {
  return (
    <main>
      <Layout pageTitle="Main Page">
        <p> This is my home page!</p>
        <StaticImage
        src="https://www.nawpic.com/media/2020/cute-cat-nawpic-7.jpg"
      />
      </Layout>
    </main>
  )
}

export const Head = () => <title>Steven</title>

export default IndexPage
