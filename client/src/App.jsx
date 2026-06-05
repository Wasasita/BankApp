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
    <div>
      <div className="app">
      <Header activePage={page} onNavigate={setPage} />

      {page === 'Home' && (
        <main className="color-black flex-1">
          <h2>Banking Frontend</h2>

          <p>
            This application provides a user interface for interacting
            with the Banking REST API.
          </p>
        </main>
      )}

      {page === 'Customers' && <Customers />}

      {page === 'Accounts' && <Accounts customerId={customerId} />}
      </div>

      <footer className="footer bg-neutral-900 text-white p-6 text-center text-sm mt-auto">
        <div className="max-w-6xl mx-auto flex flex-col sm:flex-row justify-between items-center gap-4">
          <p>&copy; {new Date().getFullYear()} Banking App. All rights reserved.</p>
          <div className="flex gap-4">
            <a href="#privacy" className="hover:underline text-neutral-400 hover:text-white transition-colors">Privacy Policy</a>
            <a href="#terms" className="hover:underline text-neutral-400 hover:text-white transition-colors">Terms of Service</a>
          </div>
        </div>
      </footer>
      
    </div>
  )
}

export default App