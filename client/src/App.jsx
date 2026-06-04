import { useState } from 'react'
import './App.css'
import Header from './Components/Header'
import Data from './pages/Customers'
import Accounts from './pages/Account'

function App() {
  const [page, setPage] = useState('Home')

  return (
    <div className="app">
      <Header activePage={page} onNavigate={setPage} />

      {page === 'Home' && (
        <main className="home-page">
          <h2>Banking Frontend</h2>

          <p>
            This application provides a user interface for interacting
            with the Banking REST API.
          </p>

          <ul>
            <li>Manage Customers</li>
            <li>Manage Accounts</li>
            <li>Search Records</li>
            <li>View Premium Customers</li>
          </ul>
        </main>
      )}

      {page === 'Customers' && <Data />}

      {page === 'Accounts' && <Accounts />}
    </div>
  )
}

export default App