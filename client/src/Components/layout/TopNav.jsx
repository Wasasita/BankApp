import { useLocation } from 'react-router-dom';

/**
 * TopNav component - Header with logo, page title, and user menu
 * @param {Object} props
 * @param {string} props.user - Current user email/name
 * @param {Function} props.onLogout - Callback for logout action
 * @returns {React.ReactNode}
 */
export function TopNav({ user, onLogout }) {
  const location = useLocation();

  const pageTitle = getPageTitle(location.pathname);

  return (
    <nav className="sticky top-0 z-50 bg-white border-b border-neutral-200 shadow-sm">
      <div className="flex items-center justify-between h-16 px-6">
        {/* Logo and App Name */}
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 bg-primary rounded-lg flex items-center justify-center">
            <span className="text-white font-bold">CB</span>
          </div>
          <h1 className="text-xl font-bold text-primary hidden sm:block">CitiBank</h1>
        </div>

        {/* Page Title - Center */}
        <div className="flex-1 text-center">
          <h2 className="text-lg font-semibold text-neutral-800">{pageTitle}</h2>
        </div>

        {/* User Menu - Right */}
        <div className="flex items-center gap-4">
          <div className="text-sm text-neutral-600">
            <p className="font-medium">{user}</p>
          </div>
          <button
            onClick={onLogout}
            className="px-4 py-2 text-sm font-medium text-primary hover:bg-primary-light rounded-lg transition-colors"
          >
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
}

/**
 * Helper function to determine page title from pathname
 */
function getPageTitle(pathname) {
  const titles = {
    '/dashboard': 'Dashboard',
    '/customers': 'Customers',
    '/accounts': 'Accounts',
    '/transactions': 'Transactions',
    '/deposit': 'Deposit',
    '/withdraw': 'Withdraw',
    '/transfer': 'Transfer',
    '/premium-customers': 'Premium Customers',
  };

  return titles[pathname] || 'Dashboard';
}
