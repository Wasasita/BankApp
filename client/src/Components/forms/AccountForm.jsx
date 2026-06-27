import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { accountSchema } from '../../utils/validation'
import { useAccounts } from '../../api/queries'

/**
 * AccountForm component - Form for creating/editing accounts
 */
export function AccountForm({ onSubmit, onCancel, initialData = null, isLoading = false }) {
  const { data: accounts = [] } = useAccounts()
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm({
    resolver: zodResolver(accountSchema),
    mode: 'onBlur',
    defaultValues: initialData || { accountNumber: '', accountType: 'Checking', balance: 0, customerId: '' },
  })

  const onSubmitHandler = async (data) => {
    try {
      await onSubmit(data)
      reset()
    } catch (error) {
      console.error('Form submission error:', error)
    }
  }

  const isSubmittingState = isLoading || isSubmitting

  return (
    <form onSubmit={handleSubmit(onSubmitHandler)} className="space-y-4">
      {/* Account Number */}
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

      {/* Account Type */}
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
          <option value="Money Market">Money Market</option>
        </select>
        {errors.accountType && (
          <p className="mt-1 text-sm text-danger">{errors.accountType.message}</p>
        )}
      </div>

      {/* Balance */}
      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Initial Balance
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

      {/* Customer ID */}
      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Customer ID
        </label>
        <input
          type="number"
          placeholder="Customer ID"
          className="input-field"
          disabled={isSubmittingState}
          {...register('customerId', { valueAsNumber: true })}
        />
        {errors.customerId && (
          <p className="mt-1 text-sm text-danger">{errors.customerId.message}</p>
        )}
      </div>

      {/* Buttons */}
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
          {isSubmittingState ? 'Saving...' : 'Save'}
        </button>
      </div>
    </form>
  )
}
