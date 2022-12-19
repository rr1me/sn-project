import './App.sass';
import {BrowserRouter, Route, Routes} from 'react-router-dom';
import ElementWrapper from "./Components/ElementWrapper/ElementWrapper";
import Home from "./Components/Home/Home";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ElementWrapper> <Home/> </ElementWrapper>}/>
        <Route path="/banlist" element={<ElementWrapper> <div>banlist</div> </ElementWrapper>}/>


        <Route path="*" element={<ElementWrapper>404?</ElementWrapper>}/>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
