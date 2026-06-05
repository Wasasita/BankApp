import { useEffect, useState } from 'react'
import DataService from '../api/DataService'
import './Account.css'

export default function Accounts({ customerId }) {
  const [accounts, setAccounts] = useState([])
  const [search, setSearch] = useState('')
  const [premiumThreshold, setPremiumThreshold] = useState('10000')
  const [message, setMessage] = useState('')
  const [loading, setLoading] = useState(true)

  const [showAddCustomer, setShowAddCustomer] = useState(false)
  const [newCustomerName, setNewCustomerName] = useState('')
  const [newCustomerEmail, setNewCustomerEmail] = useState('')
  const [createCustomerError, setCreateCustomerError] = useState('')
  const [creatingCustomer, setCreatingCustomer] = useState(false)

  const [showAddAccount, setShowAddAccount] = useState(false)
  const [newAccountCustomerId, setNewAccountCustomerId] = useState(
    customerId || ''
  )
  const [newAccountNumber, setNewAccountNumber] = useState('')
  const [newAccountType, setNewAccountType] = useState('Checking')
  const [newAccountBalance, setNewAccountBalance] = useState('0')
  const [createAccountError, setCreateAccountError] = useState('')
  const [creatingAccount, setCreatingAccount] = useState(false)

  useEffect(() => {
    let mounted = true

    const fetchAccounts = async () => {
      setLoading(true)
      setMessage('')

      try {
        const data = customerId
          ? await DataService.getAccountsByCustomerId(customerId)
          : await DataService.getAccounts()

        if (!mounted) return
        setAccounts(Array.isArray(data) ? data : [])
      } catch (err) {
        if (!mounted) return
        setMessage(err.message || 'Unable to load accounts')
      } finally {
        if (!mounted) return
        setLoading(false)
      }
    }

    fetchAccounts()

    return () => {
      mounted = false
    }
  }, [customerId])

  const handleLoadAllAccounts = async () => {
    setLoading(true)
    setMessage('')

    try {
      const data = await DataService.getAccounts()
      setAccounts(Array.isArray(data) ? data : [])
    } catch (err) {
      setMessage(err.message || 'Unable to load accounts')
    } finally {
      setLoading(false)
    }
  }

  const handleSearch = async () => {
    setMessage('')
    setLoading(true)

    if (!search.trim()) {
      await handleLoadAllAccounts()
      return
    }

    try {
      const data = await DataService.searchAccounts(search)
      const accountList = Array.isArray(data) ? data : []
      setAccounts(accountList)
      if (accountList.length === 0) {
        setMessage(`No accounts found for "${search}"`)
      }
    } catch (err) {
      setMessage(err.message || 'Search failed')
    } finally {
      setLoading(false)
    }
  }

  const handlePremiumSearch = async () => {
    setMessage('')
    setLoading(true)

    try {
      const customers = await DataService.getPremiumCustomers(
        Number(premiumThreshold)
      )

      const accountList = Array.isArray(customers)
        ? customers.flatMap((customer) =>
            (customer.accounts || []).map((account) => ({
              ...account,
              customerName: customer.name
            }))
          )
        : []

      setAccounts(accountList)
      if (accountList.length === 0) {
        setMessage(
          `No premium accounts found above $${premiumThreshold}`
        )
      }
    } catch (err) {
      setMessage(err.message || 'Premium search failed')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateCustomer = async (event) => {
    event.preventDefault()
    setCreateCustomerError('')
    setCreatingCustomer(true)

    if (!newCustomerName.trim() || !newCustomerEmail.trim()) {
      setCreateCustomerError('Name and email are required.')
      setCreatingCustomer(false)
      return
    }

    try {
      await DataService.createCustomer({
        name: newCustomerName,
        email: newCustomerEmail
      })

      setShowAddCustomer(false)
      setNewCustomerName('')
      setNewCustomerEmail('')
      setMessage('Customer created successfully. Refresh to see updates.')
    } catch (err) {
      setCreateCustomerError(err.message || 'Could not create customer')
    } finally {
      setCreatingCustomer(false)
    }
  }

  const handleCreateAccount = async (event) => {
    event.preventDefault()
    setCreateAccountError('')
    setCreatingAccount(true)

    if (!newAccountCustomerId || !newAccountNumber.trim()) {
      setCreateAccountError('Customer ID and account number are required.')
      setCreatingAccount(false)
      return
    }

    try {
      const created = await DataService.createAccount(
        {
          accountNumber: newAccountNumber,
          accountType: newAccountType,
          balance: Number(newAccountBalance || 0)
        },
        newAccountCustomerId
      )

      setAccounts((prev) => [created, ...prev])
      setShowAddAccount(false)
      setNewAccountNumber('')
      setNewAccountBalance('0')
      setCreateAccountError('')
      setMessage('Account created successfully.')
    } catch (err) {
      setCreateAccountError(err.message || 'Could not create account')
    } finally {
      setCreatingAccount(false)
    }
  }

  return (
    <div className="accounts">
      <div className="accounts-header">
        <h1>Accounts Management</h1>
        <button
          className="add-account-btn"
          onClick={() => setShowAddAccount(true)}
        >
          + Add Account
        </button>
      </div>

      <div className="search-toolbar">
        <div className="search-container">
          <input
            type="text"
            placeholder="Search by customer id"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>

        <div className="search-actions">
          <button onClick={handleSearch}>Search</button>
          <button onClick={handleLoadAllAccounts}>All Accounts</button>
        </div>
      </div>

      {/* <div className="premium-section">
        <label>Premium Search</label>
        <div className="search-bar">
          <input
            type="number"
            value={premiumThreshold}
            onChange={(e) => setPremiumThreshold(e.target.value)}
            placeholder="Threshold"
          />
          <button onClick={handlePremiumSearch}>Premium Search</button>
        </div>
      </div> */}

      {message && <p className="message">{message}</p>}
      {loading && <p>Loading accounts...</p>}

      <div className="accounts-table">
        <div className="table-header">
          <span>Account #</span>
          <span>Type</span>
          <span>Balance</span>
          {/* <span>Owner</span> */}
          <span>Actions</span>
        </div>

        {accounts.map((account) => (
          <div key={account.id} className="account-row">
            <span>{account.accountNumber}</span>
            <span>{account.accountType}</span>
            <span>${Number(account.balance || 0).toLocaleString()}</span>
            {/* <span>{account.customerName || '—'}</span> */}
            <div className="actions">
              <button>Edit</button>
              <button>Delete</button>
            </div>
          </div>
        ))}
      </div>

      {showAddCustomer && (
        <div className="modal-overlay">
          <div className="modal-card">
            <h2>Create Customer</h2>
            <form onSubmit={handleCreateCustomer}>
              <label>
                Name
                <input
                  value={newCustomerName}
                  onChange={(e) => setNewCustomerName(e.target.value)}
                  placeholder="Customer name"
                />
              </label>
              <label>
                Email
                <input
                  value={newCustomerEmail}
                  onChange={(e) => setNewCustomerEmail(e.target.value)}
                  placeholder="Customer email"
                />
              </label>
              {createCustomerError && (
                <p className="message">{createCustomerError}</p>
              )}
              <div className="modal-actions">
                <button type="submit" disabled={creatingCustomer}>
                  {creatingCustomer ? 'Creating...' : 'Create'}
                </button>
                <button
                  type="button"
                  onClick={() => setShowAddCustomer(false)}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showAddAccount && (
        <div className="modal-overlay">
          <div className="modal-card">
            <h2>Create Account</h2>
            <form onSubmit={handleCreateAccount}>
              <label>
                Customer ID
                <input
                  value={newAccountCustomerId}
                  onChange={(e) => setNewAccountCustomerId(e.target.value)}
                  placeholder="Customer ID"
                />
              </label>
              <label>
                Account Number
                <input
                  value={newAccountNumber}
                  onChange={(e) => setNewAccountNumber(e.target.value)}
                  placeholder="Account number"
                />
              </label>
              <label>
                Account Type
                <select
                  value={newAccountType}
                  onChange={(e) => setNewAccountType(e.target.value)}
                >
                  <option value="Checking">Checking</option>
                  <option value="Savings">Savings</option>
                </select>
              </label>
              <label>
                Balance
                <input
                  type="number"
                  value={newAccountBalance}
                  onChange={(e) => setNewAccountBalance(e.target.value)}
                  placeholder="Starting balance"
                />
              </label>
              {createAccountError && (
                <p className="message">{createAccountError}</p>
              )}
              <div className="modal-actions">
                <button type="submit" disabled={creatingAccount}>
                  {creatingAccount ? 'Creating...' : 'Create'}
                </button>
                <button
                  type="button"
                  onClick={() => setShowAddAccount(false)}
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