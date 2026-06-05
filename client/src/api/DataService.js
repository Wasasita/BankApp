// Simple HTTP service for talking to a Spring Boot backend.
// Configure the backend base URL with Vite env var `VITE_API_BASE`.

const BASE_URL =
  (import.meta.env && import.meta.env.VITE_API_BASE) ||
  'http://localhost:8060';

async function request(path, opts = {}) {
  const url = `${BASE_URL}${path}`
  const res = await fetch(url, opts)
  if (!res.ok) {
    const body = await res.text().catch(() => '')
    const err = new Error(`HTTP ${res.status} ${res.statusText}: ${body}`)
    err.status = res.status
    throw err
  }
  // try to parse json, fallback to text
  const ct = res.headers.get('content-type') || ''
  if (ct.includes('application/json')) return res.json()
  return res.text()
}

const DataService = {

  // Customers

  getCustomers() {
    return request('/api/customers')
  },

  getCustomer(id) {
    return request(`/api/customers/${id}`)
  },

  searchCustomers(name) {
    return request(`/api/customers/search?name=${encodeURIComponent(name)}`)
  },

  getPremiumCustomers(threshold = 0) {
    return request(
      `/api/customers/premium?threshold=${encodeURIComponent(threshold)}`
    )
  },

  getCustomerTotalBalance(id) {
    return request(`/api/customers/${id}/total-balance`)
  },

  createCustomer(customer) {
    return request('/api/customers', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(customer)
    })
  },

  updateCustomer(id, customer) {
    return request(`/api/customers/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(customer)
    })
  },

  deleteCustomer(id) {
    return request(`/api/customers/${id}`, {
      method: 'DELETE'
    })
  },

  // Accounts

  getAccounts() {
    return request('/api/accounts')
  },

  getAccount(id) {
    return request(`/api/accounts/${id}`)
  },

  searchAccounts(name) {
    return request(`/api/accounts/search?name=${encodeURIComponent(name)}`)
  },

  getAccountsByCustomerId(customerId) {
    return request(`/api/customers/${encodeURIComponent(customerId)}/accounts`)
  },

  createAccount(account, customerId) {
    return request(`/api/accounts?customerId=${encodeURIComponent(customerId)}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(account)
    })
  },

  updateAccount(id, account) {
    return request(`/api/accounts/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(account)
    })
  },

  deleteAccount(id) {
    return request(`/api/accounts/${id}`, {
      method: 'DELETE'
    })
  }
}

export default DataService