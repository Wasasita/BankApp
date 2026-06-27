// Simple HTTP service for talking to a Banking REST API.
// Configure the backend base URL with Vite env var `VITE_API_BASE`.

const BASE_URL =
  (import.meta.env && import.meta.env.VITE_API_BASE) ||
  'http://localhost:8060';

async function request(path, opts = {}) {
  const url = `${BASE_URL}${path}`
  
  // Add Authorization header if token exists
  const token = localStorage.getItem('token')
  const headers = {
    'Content-Type': 'application/json',
    ...opts.headers,
  }
  
  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  const res = await fetch(url, { ...opts, headers })
  
  // Handle 401 - unauthorized
  if (res.status === 401) {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    window.location.href = '/login'
  }
  
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

  // Authentication

  login(email, password) {
    return request('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password })
    })
  },

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

  searchCustomersByEmail(email) {
    return request(`/api/customers/search-by-email?email=${encodeURIComponent(email)}`)
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
      body: JSON.stringify(customer)
    })
  },

  updateCustomer(id, customer) {
    return request(`/api/customers/${id}`, {
      method: 'PUT',
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
      body: JSON.stringify(account)
    })
  },

  updateAccount(id, account) {
    return request(`/api/accounts/${id}`, {
      method: 'PUT',
      body: JSON.stringify(account)
    })
  },

  deleteAccount(id) {
    return request(`/api/accounts/${id}`, {
      method: 'DELETE'
    })
  },

  // Transactions

  getTransactions() {
    return request('/api/transactions')
  },

  getTransaction(id) {
    return request(`/api/transactions/${id}`)
  },

  getAccountTransactions(accountId) {
    return request(`/api/transactions/account/${accountId}`)
  },

  deposit(accountId, amount, description = '') {
    return request('/api/transactions/deposit', {
      method: 'POST',
      body: JSON.stringify({
        accountId,
        amount,
        description
      })
    })
  },

  withdraw(accountId, amount, description = '') {
    return request('/api/transactions/withdraw', {
      method: 'POST',
      body: JSON.stringify({
        accountId,
        amount,
        description
      })
    })
  },

  transfer(fromAccountId, toAccountId, amount, description = '') {
    return request('/api/transactions/transfer', {
      method: 'POST',
      body: JSON.stringify({
        fromAccountId,
        toAccountId,
        amount,
        description
      })
    })
  }
}

export default DataService