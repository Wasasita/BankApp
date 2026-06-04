import { useEffect, useState } from 'react'
import DataService from '../../api/DataService'

export default function Accounts() {

  const [accounts, setAccounts] = useState([])

  useEffect(() => {
    loadAccounts()
  }, [])

  const loadAccounts = async () => {
    const data = await DataService.getAccounts()
    setAccounts(data)
  }

  return (
    <div>
      <h2>Accounts</h2>

      {accounts.map(account => (
        <div key={account.id}>
          <p>{account.accountNumber}</p>
          <p>{account.accountType}</p>
          <p>${account.balance}</p>
        </div>
      ))}
    </div>
  )
}