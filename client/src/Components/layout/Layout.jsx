import { Outlet } from 'react-router-dom';
import { TopNav } from './TopNav';
import { Sidebar } from './Sidebar';

/**
 * Layout component - Main wrapper with navigation for authenticated pages
 * @param {Object} props
 * @param {string} props.user - Current user email/name
 * @param {Function} props.onLogout - Callback for logout action
 * @returns {React.ReactNode}
 */
export function Layout({ user, onLogout }) {
  return (
    <div className="min-h-screen bg-neutral-50">
      <TopNav user={user} onLogout={onLogout} />
      <div className="flex">
        <Sidebar />
        <main className="flex-1 overflow-auto">
          <div className="p-6 max-w-7xl mx-auto">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}
