import { useState, useEffect, useMemo } from 'react'
import { useSearchParams } from 'react-router-dom'
import {
  useAccounts,
  useSearchAccounts,
  useAccount,
  useCreateAccount,
  useUpdateAccount,
  useDeleteAccount,
} from '../api/queries'
import { Card, EmptyState } from '../Components/shared/Card'
import { LoadingSpinner } from '../Components/shared/LoadingSpinner'
import { ErrorAlert, SuccessMessage } from '../Components/shared/ErrorAlert'
import { Modal, ConfirmationModal } from '../Components/modals/Modal'
import { AccountForm } from '../Components/forms/AccountForm'
import { formatCurrency } from '../utils/formatters'

function AccountsPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const [searchQuery, setSearchQuery] = useState('')
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [editingAccount, setEditingAccount] = useState(null)
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false)
  const [accountToDelete, setAccountToDelete] = useState(null)
  const [successMessage, setSuccessMessage] = useState('')
  const [formError, setFormError] = useState('')

  const trimmedSearch = searchQuery.trim()
  const isIdSearch = /^\d+$/.test(trimmedSearch)

  const { data: allAccounts = [], isLoading, error } = useAccounts()
  const { data: searchResults = [], isFetching: isSearching } = useSearchAccounts(
    trimmedSearch && !isIdSearch ? trimmedSearch : ''
  )
  const { data: accountById, isFetching: loadingById } = useAccount(
    isIdSearch ? trimmedSearch : null
  )

  const createAccount = useCreateAccount()
  const updateAccount = useUpdateAccount()
  const deleteAccount = useDeleteAccount()

  useEffect(() => {
    if (searchParams.get('modal') === 'create') {
      setIsCreateModalOpen(true)
      setSearchParams({}, { replace: true })
    }
  }, [searchParams, setSearchParams])

  const displayedAccounts = useMemo(() => {
    if (!trimmedSearch) return allAccounts
    if (isIdSearch) return accountById ? [accountById] : []
    return searchResults
  }, [trimmedSearch, isIdSearch, allAccounts, accountById, searchResults])

  const showSuccess = (message) => {
    setSuccessMessage(message)
    setTimeout(() => setSuccessMessage(''), 3000)
  }

  const handleCreate = async (data) => {
    setFormError('')
    try {
      const { customerId, ...account } = data
      await createAccount.mutateAsync({ account, customerId })
      setIsCreateModalOpen(false)
      showSuccess('Account created successfully')
    } catch (err) {
      setFormError(err.message || 'Failed to create account')
      throw err
    }
  }

  const handleEdit = (account) => {
    setEditingAccount(account)
    setFormError('')
    setIsEditModalOpen(true)
  }

  const handleUpdate = async (data) => {
    setFormError('')
    try {
      await updateAccount.mutateAsync({ id: editingAccount.id, account: data })
      setIsEditModalOpen(false)
      setEditingAccount(null)
      showSuccess('Account updated successfully')
    } catch (err) {
      setFormError(err.message || 'Failed to update account')
      throw err
    }
  }

  const handleDeleteClick = (account) => {
    setAccountToDelete(account)
    setDeleteConfirmOpen(true)
  }

  const handleDeleteConfirm = async () => {
    await deleteAccount.mutateAsync(accountToDelete.id)
    setDeleteConfirmOpen(false)
    setAccountToDelete(null)
    showSuccess('Account deleted successfully')
  }

  if (isLoading) return <LoadingSpinner />

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-neutral-900">Accounts</h1>
          <p className="text-neutral-600 mt-1">Browse, search, create, and manage bank accounts</p>
        </div>
        <button onClick={() => setIsCreateModalOpen(true)} className="btn-primary">
          + Add Account
        </button>
      </div>

      {error && <ErrorAlert message="Failed to load accounts" />}
      {successMessage && (
        <SuccessMessage message={successMessage} onDismiss={() => setSuccessMessage('')} />
      )}

      <Card className="!shadow-none">
        <div className="space-y-3">
          <label className="block text-sm font-medium text-neutral-700">
            Search accounts by customer name or account ID
          </label>
          <div className="flex flex-col sm:flex-row gap-3">
            <input
              type="text"
              placeholder="e.g. Laura or 12"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="input-field flex-1"
            />
            {trimmedSearch && (
              <button onClick={() => setSearchQuery('')} className="btn-secondary">
                Clear
              </button>
            )}
          </div>
          {trimmedSearch && (isSearching || loadingById) && (
            <p className="text-sm text-neutral-600">Searching...</p>
          )}
        </div>
      </Card>

      {displayedAccounts.length === 0 ? (
        <EmptyState
          title="No Accounts Found"
          description={
            trimmedSearch
              ? `No accounts found for "${trimmedSearch}".`
              : 'Create your first account to get started.'
          }
          action={
            !trimmedSearch && (
              <button onClick={() => setIsCreateModalOpen(true)} className="btn-primary">
                Create Account
              </button>
            )
          }
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {displayedAccounts.map((acc) => (
            <Card
              key={acc.id}
              footer={
                <div className="flex gap-2 justify-end">
                  <button onClick={() => handleEdit(acc)} className="text-sm btn-secondary">
                    Edit
                  </button>
                  <button onClick={() => handleDeleteClick(acc)} className="text-sm btn-danger">
                    Delete
                  </button>
                </div>
              }
            >
              <div className="space-y-3">
                <div className="flex justify-between items-start">
                  <div>
                    <p className="text-xs uppercase font-bold text-neutral-500">Account #</p>
                    <p className="font-semibold text-neutral-900">{acc.accountNumber}</p>
                  </div>
                  <span className="text-xs font-medium px-2 py-1 rounded-full bg-blue-100 text-primary">
                    ID {acc.id}
                  </span>
                </div>
                <div>
                  <p className="text-xs uppercase font-bold text-neutral-500">Type</p>
                  <p className="text-neutral-800">{acc.accountType}</p>
                </div>
                {acc.customerName && (
                  <div>
                    <p className="text-xs uppercase font-bold text-neutral-500">Customer</p>
                    <p className="text-neutral-700">{acc.customerName}</p>
                  </div>
                )}
                <div>
                  <p className="text-xs uppercase font-bold text-neutral-500">Balance</p>
                  <p className="text-2xl font-bold text-primary">{formatCurrency(acc.balance)}</p>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}

      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => {
          setIsCreateModalOpen(false)
          setFormError('')
        }}
        title="Create Account"
      >
        {formError && <ErrorAlert message={formError} onClose={() => setFormError('')} />}
        <AccountForm
          mode="create"
          onSubmit={handleCreate}
          onCancel={() => {
            setIsCreateModalOpen(false)
            setFormError('')
          }}
          isLoading={createAccount.isPending}
        />
      </Modal>

      <Modal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false)
          setEditingAccount(null)
          setFormError('')
        }}
        title="Edit Account"
      >
        {formError && <ErrorAlert message={formError} onClose={() => setFormError('')} />}
        {editingAccount && (
          <AccountForm
            mode="edit"
            initialData={{
              accountNumber: editingAccount.accountNumber,
              accountType: editingAccount.accountType,
              balance: editingAccount.balance,
            }}
            onSubmit={handleUpdate}
            onCancel={() => {
              setIsEditModalOpen(false)
              setEditingAccount(null)
              setFormError('')
            }}
            isLoading={updateAccount.isPending}
          />
        )}
      </Modal>

      <ConfirmationModal
        isOpen={deleteConfirmOpen}
        onClose={() => {
          setDeleteConfirmOpen(false)
          setAccountToDelete(null)
        }}
        onConfirm={handleDeleteConfirm}
        title="Delete Account"
        message={`Delete account ${accountToDelete?.accountNumber}? This cannot be undone.`}
        confirmText="Delete"
        cancelText="Cancel"
        isDangerous
        isLoading={deleteAccount.isPending}
      />
    </div>
  )
}

export default AccountsPage
