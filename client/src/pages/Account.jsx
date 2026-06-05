import { useEffect, useState } from 'react'
import DataService from '../api/DataService'
import './Account.css'

export default function Accounts() {
  const [accounts, setAccounts] = useState([])
  const [search, setSearch] = useState('')

  useEffect(() => {
    const fetchAccounts = async () => {
      const data = await DataService.getAccounts()

      console.log(data)

      setAccounts(data)
    }

    fetchAccounts()
  }, [])

  const filteredAccounts = accounts.filter(account =>
    (account.accountNumber || '')
      .toLowerCase()
      .includes(search.toLowerCase())
  )

  return (
    <div className="accounts">

      <div className="accounts-header">
        <h1>Accounts Management</h1>

        <button className="add-account-btn">
          + Add Account
        </button>
      </div>

      <div className="search-container">
        <input
          type="text"
          placeholder="Search account number..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      <div className="accounts-table">

        <div className="table-header">
          <span>Account #</span>
          <span>Type</span>
          <span>Balance</span>
          <span>Actions</span>
        </div>

        {filteredAccounts.map(account => (
          <div
            key={account.id}
            className="account-row"
          >
            <span>
              {account.accountNumber}
            </span>

            <span>
              {account.accountType}
            </span>

            <span>
              $
              {Number(account.balance || 0)
                .toLocaleString()}
            </span>

            <div className="actions">
              <button>Edit</button>
              <button>Delete</button>
            </div>
          </div>
        ))}

      </div>

    </div>
  )
}