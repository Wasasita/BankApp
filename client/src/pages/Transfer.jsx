import { useState } from 'react'
import { useTransfer, useAccounts } from '../api/queries'
import { Card } from '../components/shared/Card'
import { SuccessMessage } from '../components/shared/ErrorAlert'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { transferSchema } from '../utils/validation'
import { formatCurrency } from '../utils/formatters'

function TransferPage() {
  const { data: accounts = [] } = useAccounts()
  const transfer = useTransfer()
  const [successMessage, setSuccessMessage] = useState('')
  const { register, handleSubmit, formState: { errors, isSubmitting }, reset, watch } = useForm({
    resolver: zodResolver(transferSchema),
    mode: 'onBlur',
  })

  const fromAccountId = watch('fromAccountId')

  const handleTransfer = async (data) => {
    try {
      await transfer.mutateAsync(data)
      setSuccessMessage('Transfer successful!')
      reset()
      setTimeout(() => setSuccessMessage(''), 3000)
    } catch (error) {
      console.error('Transfer error:', error)
    }
  }

  return (
    <div className="max-w-md mx-auto">
      <Card header={<h3 className="text-lg font-semibold">Transfer Funds</h3>}>
        {successMessage && <SuccessMessage message={successMessage} onDismiss={() => setSuccessMessage('')} />}
        <form onSubmit={handleSubmit(handleTransfer)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-2">From Account</label>
            <select className="input-field" {...register('fromAccountId', { valueAsNumber: true })}>
              <option value="">-- Select Account --</option>
              {accounts.map(acc => (
                <option key={acc.id} value={acc.id}>
                  {acc.accountNumber} ({formatCurrency(acc.balance)})
                </option>
              ))}
            </select>
            {errors.fromAccountId && <p className="text-sm text-danger">{errors.fromAccountId.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">To Account</label>
            <select className="input-field" {...register('toAccountId', { valueAsNumber: true })}>
              <option value="">-- Select Account --</option>
              {accounts.filter(a => String(a.id) !== String(fromAccountId)).map(acc => (
                <option key={acc.id} value={acc.id}>
                  {acc.accountNumber} ({formatCurrency(acc.balance)})
                </option>
              ))}
            </select>
            {errors.toAccountId && <p className="text-sm text-danger">{errors.toAccountId.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Amount</label>
            <input type="number" step="0.01" min="0.01" placeholder="0.00" className="input-field" {...register('amount', { valueAsNumber: true })} />
            {errors.amount && <p className="text-sm text-danger">{errors.amount.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Description</label>
            <textarea placeholder="Transfer reason" className="input-field" {...register('description')} />
          </div>
          <button type="submit" disabled={isSubmitting} className="btn-primary w-full">
            {isSubmitting ? 'Processing...' : 'Transfer'}
          </button>
        </form>
      </Card>
    </div>
  )
}

export default TransferPage
