import { useEffect, useState } from 'react'
// import { useNavigate } from 'react-router-dom'
import DataService from '../api/DataService'
import Customer from '../models/Customer'
import './Customers.css'

export default function Customers() {
  // const navigate = useNavigate()

  const [customers, setCustomers] = useState([])
  const [allCustomers, setAllCustomers] = useState([])

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  const [search, setSearch] = useState('')
  const [premiumThreshold, setPremiumThreshold] = useState('10000')
  const [message, setMessage] = useState('')

  useEffect(() => {
    let mounted = true

    DataService.getCustomers()
      .then((res) => {
        if (!mounted) return

        const list = Array.isArray(res)
          ? res.map((c) => Customer.from(c))
          : []

        setCustomers(list)
        setAllCustomers(list)
      })
      .catch((err) => {
        if (!mounted) return
        setError(err.message || 'Failed to load customers')
      })
      .finally(() => {
        if (!mounted) return
        setLoading(false)
      })

    return () => {
      mounted = false
    }
  }, [])

  const handleSearch = async () => {
    setMessage('')

    if (!search.trim()) {
      setCustomers(allCustomers)
      return
    }

    const results = await DataService.searchCustomers(search)

    if (!results || results.length === 0) {
      setCustomers([])
      setMessage(`No customer found with name "${search}"`)
      return
    }

    setCustomers(results)
  }

  const handlePremiumSearch = async () => {
    setMessage('')

    const results = await DataService.getPremiumCustomers(
      Number(premiumThreshold)
    )

    if (!results || results.length === 0) {
      setCustomers([])
      setMessage(
        `No premium customers found above $${premiumThreshold}`
      )
      return
    }

    setCustomers(results)
  }

  return (
    <div className="customers">
      <h1>Customer Management</h1>

      <div className="search-section">
        <h3>Search By Name</h3>

        <div className="search-bar">
          <input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Enter customer name"
          />

          <button onClick={handleSearch}>
            Search
          </button>

          <button
            onClick={() => {
              setCustomers(allCustomers)
              setMessage('')
            }}
          >
            Reset
          </button>
        </div>
      </div>

      <div className="premium-section">
        <h3 className="premium-title">
          Premium Customers
        </h3>

        <p>
          Only customers whose account balances exceed
          the specified threshold.
        </p>

        <div className="search-bar">
          <input
            type="number"
            value={premiumThreshold}
            onChange={(e) =>
              setPremiumThreshold(e.target.value)
            }
            placeholder="Minimum balance"
          />

          <button onClick={handlePremiumSearch}>
            Filter
          </button>
        </div>
      </div>

      {message && (
        <p className="message">
          {message}
        </p>
      )}

      {loading && <p>Loading customers...</p>}

      {error && (
        <p style={{ color: 'red' }}>
          Error: {error}
        </p>
      )}

      {!loading &&
        !error &&
        customers.map((c) => (
          <div
            key={c.id}
            className="customer-row"
          >
            <div className="customer-avatar">
              {(c.name || '?')
                .charAt(0)
                .toUpperCase()}
            </div>

            <div className="customer-info" >
              <p className="customer-name">
                {c.name}
              </p>

              <p className="text-sm">
                {c.email}
              </p>

              <p className="text-xs">
                ID: {c.id}
              </p>
            </div>

            <div className="customer-actions">
              <button>
                View Accounts
              </button>

              <button
                onClick={async () => {
                  await DataService.deleteCustomer(
                    c.id
                  )

                  setCustomers((prev) =>
                    prev.filter(
                      (x) => x.id !== c.id
                    )
                  )
                }}
              >
                Delete
              </button>
            </div>
          </div>
        ))}
    </div>
  )
}