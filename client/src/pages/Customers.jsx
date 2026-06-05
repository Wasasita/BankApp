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

  const [isCreateOpen, setIsCreateOpen] = useState(false)
  const [newName, setNewName] = useState('')
  const [newEmail, setNewEmail] = useState('')
  const [createError, setCreateError] = useState('')
  const [createLoading, setCreateLoading] = useState(false)

  useEffect(() => {
    let mounted = true

    const loadCustomers = async () => {
      try {
        const res = await DataService.getCustomers()
        if (!mounted) return

        const list = Array.isArray(res)
          ? await enrichCustomersWithTotal(res.map((c) => Customer.from(c)))
          : []

        setCustomers(list)
        setAllCustomers(list)
      } catch (err) {
        if (!mounted) return
        setError(err.message || 'Failed to load customers')
      } finally {
        if (!mounted) return
        setLoading(false)
      }
    }

    loadCustomers()

    return () => {
      mounted = false
    }
  }, [])

  const enrichCustomersWithTotal = async (list) => {
    const totals = await Promise.all(
      list.map(async (customer) => {
        try {
          const total = await DataService.getCustomerTotalBalance(customer.id)
          return Number(total) || 0
        } catch {
          return 0
        }
      })
    )

    return list.map((customer, index) => ({
      ...customer,
      totalBalance: totals[index]
    }))
  }

  const handleLoadAll = async () => {
    setMessage('')
    setError(null)
    setSearch('')
    setPremiumThreshold('10000')
    setLoading(true)

    try {
      const res = await DataService.getCustomers()
      const list = Array.isArray(res)
        ? await enrichCustomersWithTotal(res.map((c) => Customer.from(c)))
        : []

      setCustomers(list)
      setAllCustomers(list)
    } catch (err) {
      setError(err.message || 'Failed to load customers')
    } finally {
      setLoading(false)
    }
  }

  const handleSearch = async () => {
    setMessage('')
    setError(null)

    if (!search.trim()) {
      setCustomers(allCustomers)
      return
    }

    try {
      const results = await DataService.searchCustomers(search)
      if (!results || results.length === 0) {
        setCustomers([])
        setMessage(`No customer found with name "${search}"`)
        return
      }

      const list = Array.isArray(results)
        ? await enrichCustomersWithTotal(results.map((c) => Customer.from(c)))
        : []

      setCustomers(list)
    } catch (err) {
      setError(err.message || 'Search failed')
    }
  }

  const handlePremiumSearch = async () => {
    setMessage('')
    setError(null)

    try {
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

      const list = Array.isArray(results)
        ? await enrichCustomersWithTotal(results.map((c) => Customer.from(c)))
        : []

      setCustomers(list)
    } catch (err) {
      setError(err.message || 'Premium search failed')
    }
  }

  const handleCreateCustomer = async (event) => {
    event.preventDefault()
    setCreateError('')
    setCreateLoading(true)

    if (!newName.trim() || !newEmail.trim()) {
      setCreateError('Name and email are required.')
      setCreateLoading(false)
      return
    }

    try {
      const result = await DataService.createCustomer({
        name: newName,
        email: newEmail
      })

      const customer = Customer.from(result)
      const total = await DataService.getCustomerTotalBalance(customer.id).catch(
        () => 0
      )

      const created = { ...customer, totalBalance: Number(total) || 0 }
      setCustomers((prev) => [created, ...prev])
      setAllCustomers((prev) => [created, ...prev])
      setNewName('')
      setNewEmail('')
      setIsCreateOpen(false)
      setMessage(`Created customer ${created.name}`)
    } catch (err) {
      setCreateError(err.message || 'Failed to create customer')
    } finally {
      setCreateLoading(false)
    }
  }

  const viewAccountsForCustomer = (customerId) => {
    const params = new URLSearchParams()
    params.set('page', 'Accounts')
    params.set('customerId', customerId)
    window.open(
      `${window.location.origin}${window.location.pathname}?${params.toString()}`,
      '_blank'
    )
  }

  return (
    <div className="customers">
      <div className="customers-header">
        <h1>Customer Management</h1>

        <button
          className="primary-button"
          onClick={() => setIsCreateOpen(true)}
        >
          + Create Customer
        </button>
      </div>

      <div className="search-section">
        <h3>Search By Name</h3>

        <div className="search-bar">
          <input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Enter customer name"
          />

          <button onClick={handleSearch}>Search</button>
          <button onClick={handleLoadAll}>All Customers</button>
          <button
            onClick={() => {
              setSearch('')
              setMessage('')
              setCustomers(allCustomers)
            }}
          >
            Reset
          </button>
        </div>
      </div>

      <div className="premium-section">
        <h3 className="premium-title">Premium Customers</h3>

        <p>
          Only customers whose account balances exceed the specified
          threshold.
        </p>

        <div className="search-bar">
          <input
            type="number"
            value={premiumThreshold}
            onChange={(e) => setPremiumThreshold(e.target.value)}
            placeholder="Minimum balance"
          />

          <button onClick={handlePremiumSearch}>Filter</button>
        </div>
      </div>

      {message && <p className="message">{message}</p>}

      {loading && <p>Loading customers...</p>}

      {error && (
        <p style={{ color: 'red' }}>Error: {error}</p>
      )}

      {!loading &&
        !error &&
        customers.map((c) => (
          <div key={c.id} className="customer-row">
            <div className="customer-avatar">
              {(c.name || '?').charAt(0).toUpperCase()}
            </div>

            <div className="customer-info">
              <p className="customer-name">{c.name}</p>
              <p className="text-sm">{c.email}</p>
              <p className="text-xs">ID: {c.id}</p>
              <p className="text-xs">
                Total Balance: ${Number(c.totalBalance || 0).toLocaleString()}
              </p>
            </div>

            <div className="customer-actions">
              {/* <button onClick={() => viewAccountsForCustomer(c.id)}>
                View Accounts
              </button> */}

              <button
                onClick={async () => {
                  await DataService.deleteCustomer(c.id)
                  setCustomers((prev) => prev.filter((x) => x.id !== c.id))
                  setAllCustomers((prev) => prev.filter((x) => x.id !== c.id))
                }}
              >
                Delete
              </button>
            </div>
          </div>
        ))}

      {isCreateOpen && (
        <div className="modal-overlay">
          <div className="modal-card">
            <h2>Create Customer</h2>

            <form onSubmit={handleCreateCustomer}>
              <label>
                Name
                <input
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  placeholder="Customer name"
                />
              </label>

              <label>
                Email
                <input
                  value={newEmail}
                  onChange={(e) => setNewEmail(e.target.value)}
                  placeholder="Customer email"
                />
              </label>

              {createError && (
                <p className="message">{createError}</p>
              )}

              <div className="modal-actions">
                <button type="submit" disabled={createLoading}>
                  {createLoading ? 'Creating...' : 'Create'}
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setIsCreateOpen(false)
                    setCreateError('')
                  }}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}