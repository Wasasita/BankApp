import { useState } from 'react'
import { useCustomers, useSearchCustomers, useCreateCustomer, useUpdateCustomer, useDeleteCustomer } from '../api/queries'
import { Card, EmptyState } from '../components/shared/Card'
import { LoadingSpinner } from '../components/shared/LoadingSpinner'
import { ErrorAlert, SuccessMessage } from '../components/shared/ErrorAlert'
import { Modal } from '../components/modals/Modal'
import { ConfirmationModal } from '../components/modals/Modal'
import { CustomerForm } from '../components/forms/CustomerForm'
import { formatCurrency } from '../utils/formatters'

/**
 * Customers page - CRUD management for customers with modern UI
 */
function CustomersPage() {
  const { data: customers = [], isLoading, error } = useCustomers()
  const { data: searchResults = [], isPending: isSearching } = useSearchCustomers(searchQuery)
  const createCustomer = useCreateCustomer()
  const updateCustomer = useUpdateCustomer()
  const deleteCustomer = useDeleteCustomer()

  const [searchQuery, setSearchQuery] = useState('')
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [editingCustomer, setEditingCustomer] = useState(null)
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false)
  const [customerToDelete, setCustomerToDelete] = useState(null)
  const [successMessage, setSuccessMessage] = useState('')

  const displayedCustomers = searchQuery ? searchResults : customers

  const handleCreate = async (data) => {
    try {
      await createCustomer.mutateAsync(data)
      setIsCreateModalOpen(false)
      setSuccessMessage('Customer created successfully')
      setTimeout(() => setSuccessMessage(''), 3000)
    } catch (error) {
      console.error('Create error:', error)
    }
  }

  const handleEdit = (customer) => {
    setEditingCustomer(customer)
    setIsEditModalOpen(true)
  }

  const handleUpdate = async (data) => {
    try {
      await updateCustomer.mutateAsync({ id: editingCustomer.id, customer: data })
      setIsEditModalOpen(false)
      setEditingCustomer(null)
      setSuccessMessage('Customer updated successfully')
      setTimeout(() => setSuccessMessage(''), 3000)
    } catch (error) {
      console.error('Update error:', error)
    }
  }

  const handleDeleteClick = (customer) => {
    setCustomerToDelete(customer)
    setDeleteConfirmOpen(true)
  }

  const handleDeleteConfirm = async () => {
    try {
      await deleteCustomer.mutateAsync(customerToDelete.id)
      setDeleteConfirmOpen(false)
      setCustomerToDelete(null)
      setSuccessMessage('Customer deleted successfully')
      setTimeout(() => setSuccessMessage(''), 3000)
    } catch (error) {
      console.error('Delete error:', error)
    }
  }

  if (isLoading) return <LoadingSpinner />

  return (
    <div className="space-y-6">
      {/* Header with Search */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <h1 className="text-3xl font-bold text-neutral-900">Customers</h1>
        <button
          onClick={() => setIsCreateModalOpen(true)}
          className="btn-primary"
        >
          ➕ Add Customer
        </button>
      </div>

      {/* Messages */}
      {error && <ErrorAlert message="Failed to load customers" />}
      {successMessage && (
        <SuccessMessage message={successMessage} onDismiss={() => setSuccessMessage('')} />
      )}

      {/* Search */}
      <div>
        <input
          type="text"
          placeholder="Search customers by name..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="input-field"
        />
        {searchQuery && isSearching && <p className="mt-2 text-sm text-neutral-600">Searching...</p>}
      </div>

      {/* Customers Grid */}
      {displayedCustomers.length === 0 ? (
        <EmptyState
          icon="👥"
          title="No Customers"
          description={searchQuery ? 'No customers found matching your search' : 'No customers yet. Create your first customer to get started.'}
          action={
            !searchQuery && (
              <button
                onClick={() => setIsCreateModalOpen(true)}
                className="btn-primary"
              >
                Create Customer
              </button>
            )
          }
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {displayedCustomers.map((customer) => (
            <Card
              key={customer.id}
              footer={
                <div className="flex gap-2 justify-end">
                  <button
                    onClick={() => handleEdit(customer)}
                    className="text-sm btn-secondary"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDeleteClick(customer)}
                    className="text-sm btn-danger"
                  >
                    Delete
                  </button>
                </div>
              }
            >
              <div className="space-y-3">
                <div>
                  <p className="text-xs uppercase font-bold text-neutral-500">Name</p>
                  <p className="text-lg font-semibold text-neutral-900">{customer.name}</p>
                </div>
                <div>
                  <p className="text-xs uppercase font-bold text-neutral-500">Email</p>
                  <p className="text-neutral-700 truncate">{customer.email}</p>
                </div>
                <div>
                  <p className="text-xs uppercase font-bold text-neutral-500">Total Balance</p>
                  <p className="text-lg font-bold text-primary">
                    {formatCurrency(customer.totalBalance)}
                  </p>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}

      {/* Create Modal */}
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

      {/* Edit Modal */}
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

      {/* Delete Confirmation Modal */}
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
        isDangerous={true}
        isLoading={deleteCustomer.isPending}
      />
    </div>
  )
}

export default CustomersPage