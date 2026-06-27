import { useState } from 'react'
import { useTransactions, useAccountTransactions, useDeposit } from '../api/queries'
import { Card, EmptyState } from '../Components/shared/Card'
import { LoadingSpinner } from '../Components/shared/LoadingSpinner'
import { ErrorAlert, SuccessMessage } from '../Components/shared/ErrorAlert'
import { Modal } from '../Components/modals/Modal'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { depositSchema } from '../utils/validation'
import { useAccounts } from '../api/queries'
import { formatCurrency, formatDateTime } from '../utils/formatters'

function DepositPage() {
  const { data: accounts = [] } = useAccounts()
  const deposit = useDeposit()
  const [successMessage, setSuccessMessage] = useState('')
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm({
    resolver: zodResolver(depositSchema),
    mode: 'onBlur',
  })

  const handleDeposit = async (data) => {
    try {
      await deposit.mutateAsync(data)
      setSuccessMessage('Deposit successful!')
      reset()
      setTimeout(() => setSuccessMessage(''), 3000)
    } catch (error) {
      console.error('Deposit error:', error)
    }
  }

  return (
    <div className="max-w-md mx-auto">
      <Card header={<h3 className="text-lg font-semibold">Make a Deposit</h3>}>
        {successMessage && (
          <SuccessMessage message={successMessage} onDismiss={() => setSuccessMessage('')} />
        )}
        
        <form onSubmit={handleSubmit(handleDeposit)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-neutral-700 mb-2">Select Account</label>
            <select className="input-field" {...register('accountId', { valueAsNumber: true })}>
              <option value="">-- Select Account --</option>
              {accounts.map(acc => (
                <option key={acc.id} value={acc.id}>
                  {acc.accountNumber} ({formatCurrency(acc.balance)})
                </option>
              ))}
            </select>
            {errors.accountId && <p className="text-sm text-danger">{errors.accountId.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-neutral-700 mb-2">Amount</label>
            <input type="number" step="0.01" min="0.01" placeholder="0.00" className="input-field" {...register('amount', { valueAsNumber: true })} />
            {errors.amount && <p className="text-sm text-danger">{errors.amount.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-neutral-700 mb-2">Description</label>
            <textarea placeholder="Deposit reason" className="input-field" {...register('description')} />
          </div>

          <button type="submit" disabled={isSubmitting} className="btn-success w-full">
            {isSubmitting ? 'Processing...' : 'Deposit'}
          </button>
        </form>
      </Card>
    </div>
  )
}

export default DepositPage
