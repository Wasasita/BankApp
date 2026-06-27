import { Navigate } from 'react-router-dom';

/**
 * ProtectedRoute component to guard pages requiring authentication
 * @param {Object} props
 * @param {React.ReactNode} props.children - The component to render if authenticated
 * @param {boolean} props.isAuthenticated - Whether user is authenticated
 * @returns {React.ReactNode}
 */
export function ProtectedRoute({ children, isAuthenticated }) {
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}
