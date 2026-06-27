import { useState } from 'react'
import { useTransactions } from '../api/queries'
import { Card, EmptyState } from '../Components/shared/Card'
import { LoadingSpinner } from '../Components/shared/LoadingSpinner'
import { formatCurrency, formatDateTime, getTransactionTypeEmoji } from '../utils/formatters'

function TransactionsPage() {
  const { data: transactions = [], isLoading } = useTransactions()
  const [selectedType, setSelectedType] = useState('')

  const filtered = selectedType 
    ? transactions.filter(t => t.type === selectedType)
    : transactions

  if (isLoading) return <LoadingSpinner />

  return (
    <div className="space-y-4">
      <div className="flex gap-2">
        <button onClick={() => setSelectedType('')} className={`btn-${!selectedType ? 'primary' : 'secondary'}`}>All</button>
        <button onClick={() => setSelectedType('Deposit')} className={`btn-${selectedType === 'Deposit' ? 'primary' : 'secondary'}`}>Deposits</button>
        <button onClick={() => setSelectedType('Withdraw')} className={`btn-${selectedType === 'Withdraw' ? 'primary' : 'secondary'}`}>Withdrawals</button>
        <button onClick={() => setSelectedType('Transfer')} className={`btn-${selectedType === 'Transfer' ? 'primary' : 'secondary'}`}>Transfers</button>
      </div>

      {filtered.length === 0 ? (
        <EmptyState icon="📭" title="No Transactions" description="No transactions found" />
      ) : (
        <Card>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="border-b">
                <tr><th className="pb-3 text-left">Date</th><th className="pb-3 text-left">Type</th><th className="pb-3 text-left">Amount</th><th className="pb-3 text-left">Description</th></tr>
              </thead>
              <tbody className="divide-y">
                {filtered.map(t => (
                  <tr key={t.id} className="hover:bg-neutral-50">
                    <td className="py-3">{formatDateTime(t.date)}</td>
                    <td className="py-3 flex items-center gap-2">{getTransactionTypeEmoji(t.type)} {t.type}</td>
                    <td className="py-3 font-bold">{formatCurrency(t.amount)}</td>
                    <td className="py-3 text-neutral-600">{t.description || '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Card>
      )}
    </div>
  )
}

export default TransactionsPage
