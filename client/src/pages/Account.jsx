import { useEffect, useState } from 'react'
import DataService from '../api/DataService'
import './Account.css'

export default function Accounts() {

  const [accounts, setAccounts] = useState([])

  useEffect(() => {
    const fetchAccounts = async () => {
      const data = await DataService.getAccounts()
      setAccounts(data)
    }

    fetchAccounts()
  }, [])

  return (
    <div className="accounts">
      <h2>Accounts</h2>

      {accounts.map(account => (
        <div key={account.id} className="account-row">

          <div>
            <strong>#{account.accountNumber}</strong>
          </div>

          <div>
            {account.accountType}
          </div>

          <div>
            ${account.balance}
          </div>

        </div>
      ))}
    </div>
  )
}