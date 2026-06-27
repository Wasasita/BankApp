import { useState } from 'react'
import { useWithdraw, useAccounts } from '../api/queries'
import { Card } from '../components/shared/Card'
import { SuccessMessage } from '../components/shared/ErrorAlert'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { withdrawSchema } from '../utils/validation'
import { formatCurrency } from '../utils/formatters'

function WithdrawPage() {
  const { data: accounts = [] } = useAccounts()
  const withdraw = useWithdraw()
  const [successMessage, setSuccessMessage] = useState('')
  const { register, handleSubmit, formState: { errors, isSubmitting }, reset } = useForm({
    resolver: zodResolver(withdrawSchema),
    mode: 'onBlur',
  })

  const handleWithdraw = async (data) => {
    try {
      await withdraw.mutateAsync(data)
      setSuccessMessage('Withdrawal successful!')
      reset()
      setTimeout(() => setSuccessMessage(''), 3000)
    } catch (error) {
      console.error('Withdraw error:', error)
    }
  }

  return (
    <div className="max-w-md mx-auto">
      <Card header={<h3 className="text-lg font-semibold">Make a Withdrawal</h3>}>
        {successMessage && <SuccessMessage message={successMessage} onDismiss={() => setSuccessMessage('')} />}
        <form onSubmit={handleSubmit(handleWithdraw)} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-2">Select Account</label>
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
            <label className="block text-sm font-medium mb-2">Amount</label>
            <input type="number" step="0.01" min="0.01" placeholder="0.00" className="input-field" {...register('amount', { valueAsNumber: true })} />
            {errors.amount && <p className="text-sm text-danger">{errors.amount.message}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Description</label>
            <textarea placeholder="Withdrawal reason" className="input-field" {...register('description')} />
          </div>
          <button type="submit" disabled={isSubmitting} className="btn-danger w-full">
            {isSubmitting ? 'Processing...' : 'Withdraw'}
          </button>
        </form>
      </Card>
    </div>
  )
}

export default WithdrawPage
