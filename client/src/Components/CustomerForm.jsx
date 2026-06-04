import { useState } from 'react'

export default function CustomerForm({ onSubmit }) {

  const [name, setName] = useState('')
  const [email, setEmail] = useState('')

  const handleSubmit = (e) => {
    e.preventDefault()

    onSubmit({
      name,
      email
    })

    setName('')
    setEmail('')
  }

  return (
    <form onSubmit={handleSubmit}>
      <h3>Create Customer</h3>

      <input
        placeholder="Name"
        value={name}
        onChange={(e) => setName(e.target.value)}
      />

      <input
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />

      <button type="submit">
        Create
      </button>
    </form>
  )
}