import { useState } from 'react'
import { useAccounts } from '../api/queries'
import { Card, EmptyState } from '../Components/shared/Card'
import { LoadingSpinner } from '../Components/shared/LoadingSpinner'
import { SuccessMessage } from '../Components/shared/ErrorAlert'
import { formatCurrency } from '../utils/formatters'

function AccountsPage() {
  const { data: accounts = [], isLoading } = useAccounts()
  const [successMessage, setSuccessMessage] = useState('')

  if (isLoading) return <LoadingSpinner />

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Accounts</h1>
        <button className="btn-primary">➕ Add Account</button>
      </div>

      {successMessage && <SuccessMessage message={successMessage} onDismiss={() => setSuccessMessage('')} />}

      {accounts.length === 0 ? (
        <EmptyState icon="💳" title="No Accounts" description="Create your first account" />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {accounts.map(acc => (
            <Card key={acc.id} footer={<div className="flex gap-2"><button className="text-sm btn-secondary">Edit</button><button className="text-sm btn-danger">Delete</button></div>}>
              <div className="space-y-3">
                <div><p className="text-xs uppercase font-bold text-neutral-500">Number</p><p className="font-semibold">{acc.accountNumber}</p></div>
                <div><p className="text-xs uppercase font-bold text-neutral-500">Type</p><p>{acc.accountType}</p></div>
                <div><p className="text-xs uppercase font-bold text-neutral-500">Balance</p><p className="text-lg font-bold text-primary">{formatCurrency(acc.balance)}</p></div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}

export default AccountsPage 
//   const [accounts, setAccounts] = useState([])
//   const [search, setSearch] = useState('')
//   // const [premiumThreshold, setPremiumThreshold] = useState('10000')
//   const [message, setMessage] = useState('')
//   const [loading, setLoading] = useState(true)

//   const [showAddCustomer, setShowAddCustomer] = useState(false)
//   const [newCustomerName, setNewCustomerName] = useState('')
//   const [newCustomerEmail, setNewCustomerEmail] = useState('')
//   const [createCustomerError, setCreateCustomerError] = useState('')
//   const [creatingCustomer, setCreatingCustomer] = useState(false)

//   const [showAddAccount, setShowAddAccount] = useState(false)
//   const [newAccountCustomerId, setNewAccountCustomerId] = useState(
//     customerId || ''
//   )
//   const [newAccountNumber, setNewAccountNumber] = useState('')
//   const [newAccountType, setNewAccountType] = useState('Checking')
//   const [newAccountBalance, setNewAccountBalance] = useState('0')
//   const [createAccountError, setCreateAccountError] = useState('')
//   const [creatingAccount, setCreatingAccount] = useState(false)
//   const [editingAccount, setEditingAccount] = useState(null)

//   useEffect(() => {
//     let mounted = true

//     const fetchAccounts = async () => {
//       setLoading(true)
//       setMessage('')

//       try {
//         const data = customerId
//           ? await DataService.getAccountsByCustomerId(customerId)
//           : await DataService.getAccounts()

//         if (!mounted) return
//         setAccounts(Array.isArray(data) ? data : [])
//       } catch (err) {
//         if (!mounted) return
//         setMessage(err.message || 'Unable to load accounts')
//       } finally {
//         if (!mounted) return
//         setLoading(false)
//       }
//     }

//     fetchAccounts()

//     return () => {
//       mounted = false
//     }
//   }, [customerId])

//   const handleLoadAllAccounts = async () => {
//     setLoading(true)
//     setMessage('')

//     try {
//       const data = await DataService.getAccounts()
//       setAccounts(Array.isArray(data) ? data : [])
//     } catch (err) {
//       setMessage(err.message || 'Unable to load accounts')
//     } finally {
//       setLoading(false)
//     }
//   }

//   const handleSearch = async () => {
//     setMessage('')
//     setLoading(true)
//     if (!search.trim()) {
//       await handleLoadAllAccounts()
//       return
//     }
//     try {
//       const isId = /^\d+$/.test(search.trim())
//       let accountList

//       if (isId) {
//         const data = await DataService.getAccount(search.trim())
//         accountList = data ? [data] : []  // wrap single result in array
//       } else {
//         const data = await DataService.searchAccounts(search)
//         accountList = Array.isArray(data) ? data : []
//       }

//       setAccounts(accountList)
//       if (accountList.length === 0) {
//         setMessage(`No accounts found for "${search}"`)
//       }
//     } catch (err) {
//       setMessage(err.message || 'Search failed')
//     } finally {
//       setLoading(false)
//     }
//   }

//   const handleEditAccount = (account) => {
//     setEditingAccount(account)
//   }

//   const handleEditSubmit = async (updated) => {
//     setMessage('')
//     try {
//       await DataService.updateAccount(editingAccount.id, updated)
//       setAccounts((prev) =>
//         prev.map((a) => a.id === editingAccount.id ? { ...a, ...updated } : a)
//       )
//       setEditingAccount(null)
//       setMessage(`Account #${editingAccount.id} updated successfully.`)
//     } catch (err) {
//       setMessage(err.message || 'Could not update account')
//     }
//   }

//   const handleDeleteAccount = async (id) => {
//     if (!window.confirm(`Delete account #${id}?`)) return
//     try {
//       await DataService.deleteAccount(id)
//       setAccounts((prev) => prev.filter((a) => a.id !== id))
//       setMessage(`Account #${id} deleted.`)
//     } catch (err) {
//       setMessage(err.message || 'Delete failed')
//     }
//   }
//   // const handlePremiumSearch = async () => {
//   //   setMessage('')
//   //   setLoading(true)

//   //   try {
//   //     const customers = await DataService.getPremiumCustomers(
//   //       Number(premiumThreshold)
//   //     )

//   //     const accountList = Array.isArray(customers)
//   //       ? customers.flatMap((customer) =>
//   //           (customer.accounts || []).map((account) => ({
//   //             ...account,
//   //             customerName: customer.name
//   //           }))
//   //         )
//   //       : []

//   //     setAccounts(accountList)
//   //     if (accountList.length === 0) {
//   //       setMessage(
//   //         `No premium accounts found above $${premiumThreshold}`
//   //       )
//   //     }
//   //   } catch (err) {
//   //     setMessage(err.message || 'Premium search failed')
//   //   } finally {
//   //     setLoading(false)
//   //   }
//   // }

//   const handleCreateCustomer = async (event) => {
//     event.preventDefault()
//     setCreateCustomerError('')
//     setCreatingCustomer(true)

//     if (!newCustomerName.trim() || !newCustomerEmail.trim()) {
//       setCreateCustomerError('Name and email are required.')
//       setCreatingCustomer(false)
//       return
//     }

//     try {
//       await DataService.createCustomer({
//         name: newCustomerName,
//         email: newCustomerEmail
//       })

//       setShowAddCustomer(false)
//       setNewCustomerName('')
//       setNewCustomerEmail('')
//       setMessage('Customer created successfully. Refresh to see updates.')
//     } catch (err) {
//       setCreateCustomerError(err.message || 'Could not create customer')
//     } finally {
//       setCreatingCustomer(false)
//     }
//   }

//   const handleCreateAccount = async (event) => {
//     event.preventDefault()
//     setCreateAccountError('')
//     setCreatingAccount(true)

//     if (!newAccountCustomerId || !newAccountNumber.trim()) {
//       setCreateAccountError('Customer ID and account number are required.')
//       setCreatingAccount(false)
//       return
//     }

//     try {
//       const created = await DataService.createAccount(
//         {
//           accountNumber: newAccountNumber,
//           accountType: newAccountType,
//           balance: Number(newAccountBalance || 0)
//         },
//         newAccountCustomerId
//       )

//       setAccounts((prev) => [created, ...prev])
//       setShowAddAccount(false)
//       setNewAccountNumber('')
//       setNewAccountBalance('0')
//       setCreateAccountError('')
//       setMessage('Account created successfully.')
//     } catch (err) {
//       setCreateAccountError(err.message || 'Could not create account')
//     } finally {
//       setCreatingAccount(false)
//     }
//   }

//   return (
//     <div className="accounts">
//       <div className="accounts-header">
//         <h1>Accounts Management</h1>
//         <button
//           className="add-account-btn"
//           onClick={() => setShowAddAccount(true)}
//         >
//           + Add Account
//         </button>
//       </div>

//       <div className="search-toolbar">
//         <div className="search-container">
//           <input
//             type="text"
//             placeholder="Search by customer id or name"
//             value={search}
//             onChange={(e) => setSearch(e.target.value)}
//           />
//         </div>

//         <div className="search-actions">
//           <button onClick={handleSearch}>Search</button>
//           <button onClick={handleLoadAllAccounts}>All Accounts</button>
//         </div>
//       </div>

//       {/* <div className="premium-section">
//         <label>Premium Search</label>
//         <div className="search-bar">
//           <input
//             type="number"
//             value={premiumThreshold}
//             onChange={(e) => setPremiumThreshold(e.target.value)}
//             placeholder="Threshold"
//           />
//           <button onClick={handlePremiumSearch}>Premium Search</button>
//         </div>
//       </div> */}

//       {message && <p className="message">{message}</p>}
//       {loading && <p>Loading accounts...</p>}

//       <div className="accounts-table">
//         <div className="table-header">
//           <span>Account #</span>
//           <span>Type</span>
//           <span>Balance</span>
//           {/* <span>Owner</span> */}
//           <span>Actions</span>
//         </div>

//         {editingAccount && (
//           <div className="modal-overlay">
//             <div className="modal-card">
//               <h2>Edit Account</h2>
//               <EditAccountForm
//                 account={editingAccount}
//                 onSubmit={handleEditSubmit}
//                 onCancel={() => setEditingAccount(null)}
//               />
//             </div>
//           </div>
//         )}

//         {accounts.map((account) => (
//           <div key={account.id} className="account-row">
//             <span>{account.accountNumber}</span>
//             <span>{account.accountType}</span>
//             <span>${Number(account.balance || 0).toLocaleString()}</span>
//             {/* <span>{account.customerName || '—'}</span> */}
//             <div className="actions">
//               <button onClick={() => handleEditAccount(account)}>Edit</button>
//               <button onClick={() => handleDeleteAccount(account.id)}>Delete</button>
//             </div>
//           </div>
//         ))}
//       </div>

//       {showAddCustomer && (
//         <div className="modal-overlay">
//           <div className="modal-card">
//             <h2>Create Customer</h2>
//             <form onSubmit={handleCreateCustomer}>
//               <label>
//                 Name
//                 <input
//                   value={newCustomerName}
//                   onChange={(e) => setNewCustomerName(e.target.value)}
//                   placeholder="Customer name"
//                 />
//               </label>
//               <label>
//                 Email
//                 <input
//                   value={newCustomerEmail}
//                   onChange={(e) => setNewCustomerEmail(e.target.value)}
//                   placeholder="Customer email"
//                 />
//               </label>
//               {createCustomerError && (
//                 <p className="message">{createCustomerError}</p>
//               )}
//               <div className="modal-actions">
//                 <button type="submit" disabled={creatingCustomer}>
//                   {creatingCustomer ? 'Creating...' : 'Create'}
//                 </button>
//                 <button
//                   type="button"
//                   onClick={() => setShowAddCustomer(false)}
//                 >
//                   Cancel
//                 </button>
//               </div>
//             </form>
//           </div>
//         </div>
//       )}

//       {showAddAccount && (
//         <div className="modal-overlay">
//           <div className="modal-card">
//             <h2>Create Account</h2>
//             <form onSubmit={handleCreateAccount}>
//               <label>
//                 Customer ID
//                 <input
//                   value={newAccountCustomerId}
//                   onChange={(e) => setNewAccountCustomerId(e.target.value)}
//                   placeholder="Customer ID"
//                 />
//               </label>
//               <label>
//                 Account Number
//                 <input
//                   value={newAccountNumber}
//                   onChange={(e) => setNewAccountNumber(e.target.value)}
//                   placeholder="Account number"
//                 />
//               </label>
//               <label>
//                 Account Type
//                 <select
//                   value={newAccountType}
//                   onChange={(e) => setNewAccountType(e.target.value)}
//                 >
//                   <option value="Checking">Checking</option>
//                   <option value="Savings">Savings</option>
//                 </select>
//               </label>
//               <label>
//                 Balance
//                 <input
//                   type="number"
//                   value={newAccountBalance}
//                   onChange={(e) => setNewAccountBalance(e.target.value)}
//                   placeholder="Starting balance"
//                 />
//               </label>
//               {createAccountError && (
//                 <p className="message">{createAccountError}</p>
//               )}
//               <div className="modal-actions">
//                 <button type="submit" disabled={creatingAccount}>
//                   {creatingAccount ? 'Creating...' : 'Create'}
//                 </button>
//                 <button
//                   type="button"
//                   onClick={() => setShowAddAccount(false)}
//                 >
//                   Cancel
//                 </button>
//               </div>
//             </form>
//           </div>
//         </div>
//       )}
//     </div>
//   )
// }