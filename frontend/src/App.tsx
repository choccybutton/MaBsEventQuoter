import { BrowserRouter, Routes, Route, Link } from 'react-router-dom'
import { Dashboard, QuotesList, QuoteDetail, CustomersList, FoodItemsList } from './pages'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <nav className="app-nav">
          <div className="nav-brand">
            <h1>Catering Quotes</h1>
          </div>
          <ul className="nav-links">
            <li>
              <Link to="/">Dashboard</Link>
            </li>
            <li>
              <Link to="/quotes">Quotes</Link>
            </li>
            <li>
              <Link to="/customers">Customers</Link>
            </li>
            <li>
              <Link to="/food-items">Food Items</Link>
            </li>
          </ul>
        </nav>

        <main className="app-main">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/quotes" element={<QuotesList />} />
            <Route path="/quotes/:id" element={<QuoteDetail />} />
            <Route path="/customers" element={<CustomersList />} />
            <Route path="/food-items" element={<FoodItemsList />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  )
}

export default App
