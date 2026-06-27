import { useCustomers, useAccounts, useTransactions } from '../api/queries';
import { Card, EmptyState } from '../Components/shared/Card';
import { LoadingSpinner } from '../Components/shared/LoadingSpinner';
import { ErrorAlert } from '../Components/shared/ErrorAlert';
import { formatCurrency, formatDateTime, getTransactionTypeEmoji } from '../utils/formatters';
import { useNavigate } from 'react-router-dom';

/**
 * Dashboard page - Overview with summary cards and recent transactions
 */
export function DashboardPage() {
  const navigate = useNavigate();
  const { data: customers = [], isLoading: customersLoading, error: customersError } = useCustomers();
  const { data: accounts = [], isLoading: accountsLoading, error: accountsError } = useAccounts();
  const { data: transactions = [], isLoading: transactionsLoading, error: transactionsError } = useTransactions();

  const isLoading = customersLoading || accountsLoading || transactionsLoading;

  if (isLoading) return <LoadingSpinner />;

  // Calculate summary statistics
  const totalCustomers = customers.length;
  const totalAccounts = accounts.length;
  const totalDeposits = transactions
    .filter(t => t.type === 'Deposit')
    .reduce((sum, t) => sum + t.amount, 0);
  const totalTransactions = transactions.length;

  // Get recent transactions (last 5)
  const recentTransactions = transactions.slice(0, 5);

  return (
    <div className="space-y-6">
      {/* Errors */}
      {(customersError || accountsError || transactionsError) && (
        <ErrorAlert message="Failed to load dashboard data" />
      )}

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <SummaryCard
          title="Total Customers"
          value={totalCustomers}
          icon="👥"
          color="bg-blue-50"
        />
        <SummaryCard
          title="Total Accounts"
          value={totalAccounts}
          icon="💳"
          color="bg-green-50"
        />
        <SummaryCard
          title="Total Deposits"
          value={formatCurrency(totalDeposits)}
          icon="💰"
          color="bg-emerald-50"
        />
        <SummaryCard
          title="Total Transactions"
          value={totalTransactions}
          icon="📝"
          color="bg-purple-50"
        />
      </div>

      {/* Two Column Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Recent Transactions */}
        <div className="lg:col-span-2">
          <Card header={<h3 className="text-lg font-semibold">Recent Transactions</h3>}>
            {recentTransactions.length === 0 ? (
              <EmptyState
                icon="📭"
                title="No Transactions"
                description="No transactions have been recorded yet"
              />
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead className="border-b border-neutral-200">
                    <tr className="text-left text-neutral-600 font-medium">
                      <th className="pb-3">Date</th>
                      <th className="pb-3">Type</th>
                      <th className="pb-3">Amount</th>
                      <th className="pb-3">Description</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-neutral-100">
                    {recentTransactions.map((transaction) => (
                      <tr key={transaction.id} className="hover:bg-neutral-50">
                        <td className="py-3 text-neutral-600">
                          {formatDateTime(transaction.date)}
                        </td>
                        <td className="py-3">
                          <span className="flex items-center gap-2">
                            {getTransactionTypeEmoji(transaction.type)}
                            {transaction.type}
                          </span>
                        </td>
                        <td className="py-3 font-medium">
                          {formatCurrency(transaction.amount)}
                        </td>
                        <td className="py-3 text-neutral-600 truncate">
                          {transaction.description || '-'}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </Card>
        </div>

        {/* Quick Actions */}
        <div>
          <Card header={<h3 className="text-lg font-semibold">Quick Actions</h3>}>
            <div className="space-y-2">
              <button
                onClick={() => navigate('/customers?modal=create')}
                className="btn-primary w-full justify-center flex items-center gap-2"
              >
                ➕ New Customer
              </button>
              <button
                onClick={() => navigate('/accounts?modal=create')}
                className="btn-primary w-full justify-center flex items-center gap-2"
              >
                💳 New Account
              </button>
              <button
                onClick={() => navigate('/deposit')}
                className="btn-success w-full justify-center flex items-center gap-2"
              >
                💰 Deposit
              </button>
              <button
                onClick={() => navigate('/transfer')}
                className="btn-primary w-full justify-center flex items-center gap-2"
              >
                🔄 Transfer
              </button>
              <button
                onClick={() => navigate('/transactions')}
                className="btn-secondary w-full justify-center flex items-center gap-2"
              >
                📝 All Transactions
              </button>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
}

/**
 * SummaryCard component
 */
function SummaryCard({ title, value, icon, color }) {
  return (
    <div className={`${color} rounded-lg p-6 border border-neutral-200`}>
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-neutral-600 mb-2">{title}</p>
          <p className="text-3xl font-bold text-neutral-900">{value}</p>
        </div>
        <div className="text-3xl">{icon}</div>
      </div>
    </div>
  );
}

export default DashboardPage;
