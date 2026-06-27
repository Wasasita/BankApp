import { useLocation, useNavigate } from 'react-router-dom';
import { useState } from 'react';

/**
 * Sidebar component - Left navigation with shortcuts
 * @param {Object} props
 * @returns {React.ReactNode}
 */
export function Sidebar() {
  const navigate = useNavigate();
  const location = useLocation();
  const [isOpen, setIsOpen] = useState(true);

  const mainNavItems = [
    { path: '/dashboard', label: 'Dashboard', icon: '📊' },
    { path: '/customers', label: 'Customers', icon: '👥' },
    { path: '/accounts', label: 'Accounts', icon: '💳' },
    { path: '/transactions', label: 'Transactions', icon: '📝' },
    { path: '/premium-customers', label: 'Premium Customers', icon: '⭐' },
  ];

  // Contextual shortcuts based on current page
  const shortcuts = getShortcuts(location.pathname);

  const isActive = (path) => location.pathname === path;

  return (
    <>
      {/* Mobile toggle button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="fixed top-20 left-4 z-40 p-2 lg:hidden bg-primary text-white rounded-lg"
      >
        {isOpen ? '✕' : '☰'}
      </button>

      {/* Sidebar */}
      <aside
        className={`${
          isOpen ? 'translate-x-0' : '-translate-x-full'
        } fixed left-0 top-16 bottom-0 w-64 bg-white border-r border-neutral-200 overflow-y-auto transition-transform duration-300 lg:translate-x-0 lg:static lg:top-auto z-30`}
      >
        <div className="p-4">
          {/* Main Navigation */}
          <nav className="space-y-2 mb-8">
            <h3 className="text-xs uppercase font-bold text-neutral-500 px-4 mb-2">
              Navigation
            </h3>
            {mainNavItems.map((item) => (
              <button
                key={item.path}
                onClick={() => {
                  navigate(item.path);
                  setIsOpen(false);
                }}
                className={`w-full text-left px-4 py-3 rounded-lg font-medium transition-colors flex items-center gap-3 ${
                  isActive(item.path)
                    ? 'bg-primary-light text-primary'
                    : 'text-neutral-700 hover:bg-neutral-100'
                }`}
              >
                <span className="text-lg">{item.icon}</span>
                <span>{item.label}</span>
              </button>
            ))}
          </nav>

          {/* Contextual Shortcuts */}
          {shortcuts.length > 0 && (
            <div>
              <h3 className="text-xs uppercase font-bold text-neutral-500 px-4 mb-2">
                Quick Actions
              </h3>
              <div className="space-y-2">
                {shortcuts.map((shortcut) => (
                  <button
                    key={shortcut.path}
                    onClick={() => {
                      navigate(shortcut.path);
                      setIsOpen(false);
                    }}
                    className="w-full text-left px-4 py-2 rounded-lg text-sm font-medium text-primary bg-primary-light hover:bg-blue-200 transition-colors flex items-center gap-2"
                  >
                    <span>{shortcut.icon}</span>
                    <span>{shortcut.label}</span>
                  </button>
                ))}
              </div>
            </div>
          )}
        </div>
      </aside>

      {/* Mobile overlay */}
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/50 top-16 lg:hidden z-20"
          onClick={() => setIsOpen(false)}
        />
      )}
    </>
  );
}

/**
 * Determine contextual shortcuts based on current page
 */
function getShortcuts(pathname) {
  const shortcuts = {
    '/customers': [
      { path: '/customers?modal=create', label: 'Add Customer', icon: '➕' },
      { path: '/customers?search=true', label: 'Search Customer', icon: '🔍' },
      { path: '/premium-customers', label: 'Premium Customers', icon: '⭐' },
    ],
    '/accounts': [
      { path: '/accounts?modal=create', label: 'Add Account', icon: '➕' },
      { path: '/accounts?search=true', label: 'Search Account', icon: '🔍' },
    ],
    '/transactions': [
      { path: '/deposit', label: 'Deposit', icon: '💰' },
      { path: '/withdraw', label: 'Withdraw', icon: '💸' },
      { path: '/transfer', label: 'Transfer', icon: '🔄' },
    ],
    '/dashboard': [
      { path: '/customers?modal=create', label: 'New Customer', icon: '➕' },
      { path: '/accounts?modal=create', label: 'New Account', icon: '💳' },
      { path: '/deposit', label: 'Deposit', icon: '💰' },
      { path: '/transfer', label: 'Transfer', icon: '🔄' },
    ],
  };

  return shortcuts[pathname] || [];
}
