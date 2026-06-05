import { useState } from 'react'
import './App.css'
import Header from './Components/Header'
import Accounts from './pages/Account'
import Customers from './pages/Customers'

function App() {
  const searchParams = new URLSearchParams(window.location.search)
  const initialCustomerId = searchParams.get('customerId')
  const initialPage =
    searchParams.get('page') ||
    (initialCustomerId ? 'Accounts' : 'Home')

  const [page, setPage] = useState(initialPage)
  const customerId = initialCustomerId
    ? Number(initialCustomerId)
    : undefined

  return (
    <div className="app">
      <Header activePage={page} onNavigate={setPage} />

      {page === 'Home' && (
        <main className="color-black">
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

      {page === 'Customers' && <Customers />}

      {page === 'Accounts' && <Accounts customerId={customerId} />}
    </div>
  )
}

export default App