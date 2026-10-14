import { NavLink, Route, Routes } from 'react-router-dom';
import Home from './pages/Home';
import Buy from './pages/Buy';
import Manage from './pages/Manage';
import Admin from './pages/Admin';
import Reports from './pages/Reports';
export default function App() {
    return (<><header>
        <h1>Qatar 2027</h1>
        <nav>
            <NavLink to="/">Početna</NavLink>
            <NavLink to="/buy">Kupovina</NavLink>
            <NavLink to="/manage">Izmena/Otkazivanje</NavLink>
            <NavLink to="/admin">Administracija</NavLink>
            <NavLink to="/reports">Izveštaji</NavLink>
        </nav>
    </header>
        <main>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/buy" element={<Buy />} />
                <Route path="/manage" element={<Manage />} />
                <Route path="/admin" element={<Admin />} />
                <Route path="/reports" element={<Reports />} />
            </Routes>
        </main>
    </>);
};