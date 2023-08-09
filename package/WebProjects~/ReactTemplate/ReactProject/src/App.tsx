import React, { useRef } from 'react'
import "@needle-tools/engine";

export default function App() {
  return (
    <>
      <NeedleEngine>
        <div>
          <h1>Needle Engine</h1>
          <button>Click me!</button>
        </div>
      </NeedleEngine>
    </>
  )
}

// TODO add events, e.g. https://coryrylan.com/blog/how-to-use-web-components-with-typescript-and-react
function NeedleEngine(props) {
    return <needle-engine>
        {props.children}
    </needle-engine>;
}

// TODO seems to break syntax highlighting in <div> etc above
declare global {
  namespace JSX {
    interface IntrinsicElements {
      'needle-engine': React.DetailedHTMLProps<React.HTMLAttributes<HTMLElement>, HTMLElement>;
    }
  }
}