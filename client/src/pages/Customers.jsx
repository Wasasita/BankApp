import { useState, useEffect, useMemo } from 'react'
import { useSearchParams } from 'react-router-dom'
import {
  useCustomers,
  useSearchCustomers,
  useSearchCustomersByEmail,
  usePremiumCustomers,
  useCreateCustomer,
  useUpdateCustomer,
  useDeleteCustomer,
} from '../api/queries'
import { Card, EmptyState } from '../Components/shared/Card'
import { LoadingSpinner } from '../Components/shared/LoadingSpinner'
import { ErrorAlert, SuccessMessage } from '../Components/shared/ErrorAlert'
import { Modal, ConfirmationModal } from '../Components/modals/Modal'
import { CustomerForm } from '../Components/forms/CustomerForm'
import { formatCurrency } from '../utils/formatters'

const VIEW_TABS = [
  { id: 'all', label: 'All Customers'},
  { id: 'premium', label: 'Premium'},
  { id: 'standard', label: 'Standard'},
]

const SEARCH_MODES = [
  { id: 'name', label: 'By Name', placeholder: 'Search by customer name...' },
  { id: 'email', label: 'By Email', placeholder: 'Search by email address...' },
]

function CustomerCard({ customer, isPremium, onEdit, onDelete }) {
  return (
    <Card
      footer={
        <div className="flex gap-2 justify-end">
          <button onClick={() => onEdit(customer)} className="text-sm btn-secondary">
            Edit
          </button>
          <button onClick={() => onDelete(customer)} className="text-sm btn-danger">
            Delete
          </button>
        </div>
      }
    >
      <div className="space-y-3">
        <div className="flex items-start justify-between gap-2">
          <div>
            <p className="text-xs uppercase font-bold text-neutral-500">Name</p>
            <p className="text-lg font-semibold text-neutral-900">{customer.name}</p>
          </div>
          {isPremium && (
            <span className="text-xs font-semibold px-2 py-1 rounded-full bg-amber-100 text-amber-800">
              Premium
            </span>
          )}
        </div>
        <div>
          <p className="text-xs uppercase font-bold text-neutral-500">Email</p>
          <p className="text-neutral-700 truncate">{customer.email}</p>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <p className="text-xs uppercase font-bold text-neutral-500">Total Balance</p>
            <p className="text-lg font-bold text-primary">{formatCurrency(customer.totalBalance)}</p>
          </div>
          <div>
            <p className="text-xs uppercase font-bold text-neutral-500">Accounts</p>
            <p className="text-lg font-semibold text-neutral-800">{customer.accountsCount}</p>
          </div>
        </div>
      </div>
    </Card>
  )
}

function CustomersPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const [viewTab, setViewTab] = useState('all')
  const [searchMode, setSearchMode] = useState('name')
  const [searchQuery, setSearchQuery] = useState('')
  const [premiumThreshold, setPremiumThreshold] = useState(10000)
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [editingCustomer, setEditingCustomer] = useState(null)
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false)
  const [customerToDelete, setCustomerToDelete] = useState(null)
  const [successMessage, setSuccessMessage] = useState('')

  const { data: customers = [], isLoading, error } = useCustomers()
  const trimmedSearch = searchQuery.trim()
  const { data: searchByName = [], isFetching: searchingName } = useSearchCustomers(
    searchMode === 'name' ? trimmedSearch : ''
  )
  const { data: searchByEmail = [], isFetching: searchingEmail } = useSearchCustomersByEmail(
    searchMode === 'email' ? trimmedSearch : ''
  )
  const { data: premiumCustomers = [] } = usePremiumCustomers(premiumThreshold, true)

  const createCustomer = useCreateCustomer()
  const updateCustomer = useUpdateCustomer()
  const deleteCustomer = useDeleteCustomer()

  useEffect(() => {
    if (searchParams.get('modal') === 'create') {
      setIsCreateModalOpen(true)
      setSearchParams({}, { replace: true })
    }
  }, [searchParams, setSearchParams])

  const premiumIds = useMemo(
    () => new Set(premiumCustomers.map((customer) => customer.id)),
    [premiumCustomers]
  )

  const displayedCustomers = useMemo(() => {
    if (trimmedSearch) {
      return searchMode === 'name' ? searchByName : searchByEmail
    }

    if (viewTab === 'premium') return premiumCustomers
    if (viewTab === 'standard') {
      return customers.filter((customer) => !premiumIds.has(customer.id))
    }

    return customers
  }, [
    trimmedSearch,
    searchMode,
    searchByName,
    searchByEmail,
    viewTab,
    premiumCustomers,
    customers,
    premiumIds,
  ])

  const isSearching = trimmedSearch && (searchingName || searchingEmail)
  const pageLoading = isLoading

  const showSuccess = (message) => {
    setSuccessMessage(message)
    setTimeout(() => setSuccessMessage(''), 3000)
  }

  const handleCreate = async (data) => {
    await createCustomer.mutateAsync(data)
    setIsCreateModalOpen(false)
    showSuccess('Customer created successfully')
  }

  const handleEdit = (customer) => {
    setEditingCustomer(customer)
    setIsEditModalOpen(true)
  }

  const handleUpdate = async (data) => {
    await updateCustomer.mutateAsync({ id: editingCustomer.id, customer: data })
    setIsEditModalOpen(false)
    setEditingCustomer(null)
    showSuccess('Customer updated successfully')
  }

  const handleDeleteClick = (customer) => {
    setCustomerToDelete(customer)
    setDeleteConfirmOpen(true)
  }

  const handleDeleteConfirm = async () => {
    await deleteCustomer.mutateAsync(customerToDelete.id)
    setDeleteConfirmOpen(false)
    setCustomerToDelete(null)
    showSuccess('Customer deleted successfully')
  }

  if (pageLoading) return <LoadingSpinner />

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-neutral-900">Customers</h1>
          <p className="text-neutral-600 mt-1">Manage customers, search, and filter premium accounts</p>
        </div>
        <button onClick={() => setIsCreateModalOpen(true)} className="btn-primary">
          + Add Customer
        </button>
      </div>

      {error && <ErrorAlert message="Failed to load customers" />}
      {successMessage && (
        <SuccessMessage message={successMessage} onDismiss={() => setSuccessMessage('')} />
      )}

      <Card className="!shadow-none">
        <div className="space-y-4">
          <div className="flex flex-wrap gap-2">
            {VIEW_TABS.map((tab) => (
              <button
                key={tab.id}
                onClick={() => {
                  setViewTab(tab.id)
                  setSearchQuery('')
                }}
                className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
                  viewTab === tab.id && !trimmedSearch
                    ? 'bg-primary text-white'
                    : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
                }`}
              >
                {tab.icon} {tab.label}
              </button>
            ))}
          </div>

          {(viewTab === 'premium' || viewTab === 'standard') && !trimmedSearch && (
            <div className="flex flex-col sm:flex-row sm:items-end gap-3 p-4 bg-amber-50 border border-amber-200 rounded-lg">
              <div className="flex-1">
                <label className="block text-sm font-medium text-neutral-700 mb-2">
                  Premium balance threshold
                </label>
                <input
                  type="number"
                  min="0"
                  step="1000"
                  value={premiumThreshold}
                  onChange={(e) => setPremiumThreshold(Number(e.target.value) || 0)}
                  className="input-field"
                />
              </div>
              <p className="text-sm text-neutral-600 pb-2">
                Customers with total balance ≥ {formatCurrency(premiumThreshold)} are premium.
              </p>
            </div>
          )}

          <div className="flex flex-wrap gap-2">
            {SEARCH_MODES.map((mode) => (
              <button
                key={mode.id}
                onClick={() => setSearchMode(mode.id)}
                className={`px-3 py-1.5 rounded-md text-sm ${
                  searchMode === mode.id
                    ? 'bg-blue-100 text-primary font-medium'
                    : 'text-neutral-600 hover:bg-neutral-100'
                }`}
              >
                {mode.label}
              </button>
            ))}
          </div>

          <input
            type={searchMode === 'email' ? 'email' : 'text'}
            placeholder={SEARCH_MODES.find((m) => m.id === searchMode)?.placeholder}
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="input-field"
          />
          {isSearching && <p className="text-sm text-neutral-600">Searching...</p>}
        </div>
      </Card>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <Card className="!p-4 bg-blue-50 border-blue-100">
          <p className="text-sm text-neutral-600">Showing</p>
          <p className="text-2xl font-bold text-neutral-900">{displayedCustomers.length}</p>
        </Card>
        <Card className="!p-4 bg-amber-50 border-amber-100">
          <p className="text-sm text-neutral-600">Premium (≥ {formatCurrency(premiumThreshold)})</p>
          <p className="text-2xl font-bold text-amber-800">{premiumCustomers.length}</p>
        </Card>
        <Card className="!p-4 bg-neutral-50">
          <p className="text-sm text-neutral-600">Standard</p>
          <p className="text-2xl font-bold text-neutral-900">
            {Math.max(customers.length - premiumCustomers.length, 0)}
          </p>
        </Card>
      </div>

      {displayedCustomers.length === 0 ? (
        <EmptyState
          title="No Customers Found"
          description={
            trimmedSearch
              ? 'No customers match your search. Try a different name or email.'
              : viewTab === 'premium'
                ? `No premium customers above ${formatCurrency(premiumThreshold)}.`
                : 'No customers yet. Create your first customer to get started.'
          }
          action={
            !trimmedSearch && viewTab === 'all' && (
              <button onClick={() => setIsCreateModalOpen(true)} className="btn-primary">
                Create Customer
              </button>
            )
          }
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {displayedCustomers.map((customer) => (
            <CustomerCard
              key={customer.id}
              customer={customer}
              isPremium={premiumIds.has(customer.id)}
              onEdit={handleEdit}
              onDelete={handleDeleteClick}
            />
          ))}
        </div>
      )}

      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        title="Create Customer"
      >
        <CustomerForm
          onSubmit={handleCreate}
          onCancel={() => setIsCreateModalOpen(false)}
          isLoading={createCustomer.isPending}
        />
      </Modal>

      <Modal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false)
          setEditingCustomer(null)
        }}
        title="Edit Customer"
      >
        {editingCustomer && (
          <CustomerForm
            initialData={editingCustomer}
            onSubmit={handleUpdate}
            onCancel={() => {
              setIsEditModalOpen(false)
              setEditingCustomer(null)
            }}
            isLoading={updateCustomer.isPending}
          />
        )}
      </Modal>

      <ConfirmationModal
        isOpen={deleteConfirmOpen}
        onClose={() => {
          setDeleteConfirmOpen(false)
          setCustomerToDelete(null)
        }}
        onConfirm={handleDeleteConfirm}
        title="Delete Customer"
        message={`Are you sure you want to delete ${customerToDelete?.name}? This action cannot be undone.`}
        confirmText="Delete"
        cancelText="Cancel"
        isDangerous
        isLoading={deleteCustomer.isPending}
      />
    </div>
  )
}

export default CustomersPage
