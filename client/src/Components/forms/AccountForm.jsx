import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { accountSchema, accountEditSchema } from '../../utils/validation'
import { useCustomers } from '../../api/queries'

/**
 * AccountForm component - Form for creating/editing accounts
 */
export function AccountForm({ onSubmit, onCancel, initialData = null, isLoading = false, mode = 'create' }) {
  const { data: customers = [] } = useCustomers()
  const isEdit = mode === 'edit'
  const schema = isEdit ? accountEditSchema : accountSchema

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm({
    resolver: zodResolver(schema),
    mode: 'onBlur',
    defaultValues: initialData || {
      accountNumber: '',
      accountType: 'Checking',
      balance: 0,
      customerId: undefined,
    },
  })

  const onSubmitHandler = async (data) => {
    try {
      await onSubmit(data)
      if (!isEdit) reset()
    } catch (error) {
      console.error('Form submission error:', error)
    }
  }

  const isSubmittingState = isLoading || isSubmitting

  return (
    <form onSubmit={handleSubmit(onSubmitHandler)} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Account Number
        </label>
        <input
          type="text"
          placeholder="ACC123456"
          className="input-field"
          disabled={isSubmittingState}
          {...register('accountNumber')}
        />
        {errors.accountNumber && (
          <p className="mt-1 text-sm text-danger">{errors.accountNumber.message}</p>
        )}
      </div>

      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Account Type
        </label>
        <select
          className="input-field"
          disabled={isSubmittingState}
          {...register('accountType')}
        >
          <option value="Checking">Checking</option>
          <option value="Savings">Savings</option>
        </select>
        {errors.accountType && (
          <p className="mt-1 text-sm text-danger">{errors.accountType.message}</p>
        )}
      </div>

      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          {isEdit ? 'Balance' : 'Initial Balance'}
        </label>
        <input
          type="number"
          placeholder="0.00"
          step="0.01"
          min="0"
          className="input-field"
          disabled={isSubmittingState}
          {...register('balance', { valueAsNumber: true })}
        />
        {errors.balance && (
          <p className="mt-1 text-sm text-danger">{errors.balance.message}</p>
        )}
      </div>

      {!isEdit && (
        <div>
          <label className="block text-sm font-medium text-neutral-700 mb-2">
            Customer
          </label>
          <select
            className="input-field"
            disabled={isSubmittingState}
            {...register('customerId', { valueAsNumber: true })}
          >
            <option value="">Select a customer</option>
            {customers.map((customer) => (
              <option key={customer.id} value={customer.id}>
                {customer.name} ({customer.email})
              </option>
            ))}
          </select>
          {errors.customerId && (
            <p className="mt-1 text-sm text-danger">{errors.customerId.message}</p>
          )}
        </div>
      )}

      <div className="flex gap-3 justify-end">
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmittingState}
          className="btn-secondary disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isSubmittingState}
          className="btn-primary disabled:opacity-50"
        >
          {isSubmittingState ? 'Saving...' : isEdit ? 'Update Account' : 'Create Account'}
        </button>
      </div>
    </form>
  )
}
