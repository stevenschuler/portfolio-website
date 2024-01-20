import * as React from 'react'

const Frame = (props) => {
    return (
        <>
        <h1>painting is called {props.name}</h1>
        {props.children}
        </> 
    )
}

const DevPage = () => {
    const paintingName = "yuhr"
    return(
        <main>
            <Frame name={paintingName}>
                <p>{paintingName} is a fantastic painting</p>
            </Frame>

        </main>
    )
}

export const Head = () => <title>Dev Page</title>

export default DevPage