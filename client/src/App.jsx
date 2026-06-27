import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider, useAuth } from './context/AuthContext';
import { Layout } from './Components/layout/Layout'; // component folder must use lowercase
import { ProtectedRoute } from './Components/layout/ProtectedRoute';
import LoginPage from './pages/Login';
import DashboardPage from './pages/Dashboard';
import CustomersPage from './pages/Customers';
import AccountsPage from './pages/Account';
import TransactionsPage from './pages/Transactions';
import DepositPage from './pages/Deposit';
import WithdrawPage from './pages/Withdraw';
import TransferPage from './pages/Transfer';
import PremiumCustomersPage from './pages/PremiumCustomers';

// Create Query Client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      gcTime: 1000 * 60 * 10, // 10 minutes (formerly cacheTime)
    },
  },
});

/**
 * AppRoutes component - Routes that depend on Auth context
 */
function AppRoutes() {
  const auth = useAuth();
  
  if (auth.isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="w-8 h-8 border-4 border-neutral-200 border-t-primary rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/login" element={<LoginPage />} />

      {/* Protected Routes */}
      <Route
        element={
          <ProtectedRoute isAuthenticated={auth.isAuthenticated}>
            <Layout user={auth.user?.email || auth.user || 'User'} onLogout={auth.logout} />
          </ProtectedRoute>
        }
      >
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/customers" element={<CustomersPage />} />
        <Route path="/accounts" element={<AccountsPage />} />
        <Route path="/transactions" element={<TransactionsPage />} />
        <Route path="/deposit" element={<DepositPage />} />
        <Route path="/withdraw" element={<WithdrawPage />} />
        <Route path="/transfer" element={<TransferPage />} />
        <Route path="/premium-customers" element={<PremiumCustomersPage />} />
      </Route>

      {/* Default Route */}
      <Route
        path="/"
        element={<Navigate to={auth.isAuthenticated ? '/dashboard' : '/login'} replace />}
      />
    </Routes>
  );
}

/**
 * Main App component
 */
function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          <AppRoutes />
        </AuthProvider>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;