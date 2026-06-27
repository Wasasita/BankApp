import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useCustomers, usePremiumCustomers } from '../api/queries'
import { Card, EmptyState } from '../Components/shared/Card'
import { LoadingSpinner } from '../Components/shared/LoadingSpinner'
import { ErrorAlert } from '../Components/shared/ErrorAlert'
import { formatCurrency } from '../utils/formatters'

function PremiumCustomersPage() {
  const [threshold, setThreshold] = useState(10000)
  const { data: allCustomers = [], isLoading: loadingAll, error: allError } = useCustomers()
  const { data: premiumCustomers = [], isLoading: loadingPremium, error: premiumError } =
    usePremiumCustomers(threshold)

  const premiumIds = new Set(premiumCustomers.map((customer) => customer.id))
  const standardCustomers = allCustomers.filter((customer) => !premiumIds.has(customer.id))
  const isLoading = loadingAll || loadingPremium
  const error = allError || premiumError

  if (isLoading) return <LoadingSpinner />

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-neutral-900">Premium Customers</h1>
        <p className="text-neutral-600 mt-1">
          Compare premium and standard customers using the balance threshold API
        </p>
      </div>

      {error && <ErrorAlert message="Failed to load premium customer data" />}

      <Card className="!shadow-none bg-gradient-to-r from-amber-50 to-orange-50 border-amber-200">
        <div className="flex flex-col lg:flex-row lg:items-end gap-4">
          <div className="flex-1">
            <label className="block text-sm font-medium text-neutral-700 mb-2">
              Premium threshold (GET /api/customers/premium)
            </label>
            <input
              type="number"
              min="0"
              step="1000"
              value={threshold}
              onChange={(e) => setThreshold(Number(e.target.value) || 0)}
              className="input-field max-w-xs"
            />
          </div>
          <p className="text-sm text-neutral-600">
            Customers with combined account balance ≥ {formatCurrency(threshold)} qualify as premium.
          </p>
        </div>
      </Card>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <Card className="!p-4 bg-amber-50 border-amber-200">
          <p className="text-sm text-neutral-600">Premium customers</p>
          <p className="text-3xl font-bold text-amber-800">{premiumCustomers.length}</p>
        </Card>
        <Card className="!p-4 bg-neutral-50">
          <p className="text-sm text-neutral-600">Standard customers</p>
          <p className="text-3xl font-bold text-neutral-900">{standardCustomers.length}</p>
        </Card>
        <Card className="!p-4 bg-blue-50 border-blue-100">
          <p className="text-sm text-neutral-600">Total customers</p>
          <p className="text-3xl font-bold text-primary">{allCustomers.length}</p>
        </Card>
      </div>

      <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
        <section className="space-y-4">
          <h2 className="text-xl font-semibold text-neutral-900 flex items-center gap-2">
            Premium ({premiumCustomers.length})
          </h2>
          {premiumCustomers.length === 0 ? (
            <EmptyState
              title="No Premium Customers"
              description={`No customers meet the ${formatCurrency(threshold)} threshold.`}
            />
          ) : (
            <div className="space-y-3">
              {premiumCustomers.map((customer) => (
                <Card key={customer.id} className="!p-4 border-amber-200 bg-amber-50/50">
                  <div className="flex justify-between items-start gap-4">
                    <div>
                      <p className="font-semibold text-neutral-900">{customer.name}</p>
                      <p className="text-sm text-neutral-600">{customer.email}</p>
                      <p className="text-xs text-neutral-500 mt-1">
                        {customer.accountsCount} account{customer.accountsCount !== 1 ? 's' : ''}
                      </p>
                    </div>
                    <p className="text-lg font-bold text-amber-800">
                      {formatCurrency(customer.totalBalance)}
                    </p>
                  </div>
                </Card>
              ))}
            </div>
          )}
        </section>

        <section className="space-y-4">
          <h2 className="text-xl font-semibold text-neutral-900 flex items-center gap-2">
            Standard ({standardCustomers.length})
          </h2>
          {standardCustomers.length === 0 ? (
            <EmptyState
              title="No Standard Customers"
              description="All customers currently qualify as premium at this threshold."
            />
          ) : (
            <div className="space-y-3 max-h-[32rem] overflow-y-auto pr-1">
              {standardCustomers.map((customer) => (
                <Card key={customer.id} className="!p-4">
                  <div className="flex justify-between items-start gap-4">
                    <div>
                      <p className="font-semibold text-neutral-900">{customer.name}</p>
                      <p className="text-sm text-neutral-600">{customer.email}</p>
                      <p className="text-xs text-neutral-500 mt-1">
                        {customer.accountsCount} account{customer.accountsCount !== 1 ? 's' : ''}
                      </p>
                    </div>
                    <p className="text-lg font-bold text-neutral-800">
                      {formatCurrency(customer.totalBalance)}
                    </p>
                  </div>
                </Card>
              ))}
            </div>
          )}
        </section>
      </div>

      <Card className="!p-4 bg-blue-50 border-blue-100">
        <p className="text-sm text-neutral-700">
          Need name or email search? Visit{' '}
          <Link to="/customers" className="text-primary font-medium hover:underline">
            Customers
          </Link>{' '}
          for full CRUD and search tools.
        </p>
      </Card>
    </div>
  )
}

export default PremiumCustomersPage
