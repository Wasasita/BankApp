import { useState } from 'react'

export default function AccountForm({ onSubmit }) {

  const [accountNumber, setAccountNumber] = useState('')
  const [accountType, setAccountType] = useState('Savings')
  const [balance, setBalance] = useState(0)
  const [customerId, setCustomerId] = useState('')

  const handleSubmit = (e) => {
    e.preventDefault()

    onSubmit({
      accountNumber,
      accountType,
      balance,
      customerId
    })
  }

  return (
    <form onSubmit={handleSubmit}>
      <h3>Create Account</h3>

      <input
        placeholder="Customer ID"
        value={customerId}
        onChange={(e) => setCustomerId(e.target.value)}
      />

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

      <button type="submit">
        Create
      </button>
    </form>
  )
}