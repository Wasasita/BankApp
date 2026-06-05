import { useState } from 'react'

export default function EditAccountForm({ account, onSubmit, onCancel }) {
  const [accountNumber, setAccountNumber] = useState(account.accountNumber)
  const [accountType, setAccountType] = useState(account.accountType)
  const [balance, setBalance] = useState(account.balance)

  const handleSubmit = (e) => {
    e.preventDefault()
    onSubmit({ accountNumber, accountType, balance: Number(balance) })
  }

  return (
    <form onSubmit={handleSubmit}>
      <h3>Edit Account #{account.id}</h3>

      <input
        placeholder="Account Number"
        value={accountNumber}
        onChange={(e) => setAccountNumber(e.target.value)}
      />

      <select
        value={accountType}
        onChange={(e) => setAccountType(e.target.value)}
      >
        <option>Savings</option>
        <option>Checking</option>
      </select>

      <input
        type="number"
        value={balance}
        onChange={(e) => setBalance(e.target.value)}
      />

      <button type="submit">Save</button>
      <button type="button" onClick={onCancel}>Cancel</button>
    </form>
  )
}