import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LoginForm } from '../components/forms/LoginForm';

/**
 * Login page
 */
export function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async (credentials) => {
    setIsLoading(true);
    try {
      const result = await login(credentials.email, credentials.password);
      
      if (result.success) {
        navigate('/dashboard', { replace: true });
      } else {
        throw new Error(result.error || 'Login failed');
      }
    } catch (error) {
      throw error;
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-primary-light flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Card */}
        <div className="bg-white rounded-lg shadow-lg p-8">
          {/* Header */}
          <div className="text-center mb-8">
            <div className="w-16 h-16 bg-primary rounded-lg flex items-center justify-center mx-auto mb-4">
              <span className="text-3xl text-white font-bold">CB</span>
            </div>
            <h1 className="text-2xl font-bold text-neutral-900 mb-2">CitiBank</h1>
            <p className="text-neutral-600">Modern Banking Dashboard</p>
          </div>

          {/* Form */}
          <LoginForm onSubmit={handleLogin} isLoading={isLoading} />
        </div>

        {/* Footer */}
        <div className="text-center mt-6 text-sm text-neutral-600">
          <p>© {new Date().getFullYear()} CitiBank. All rights reserved.</p>
        </div>
      </div>
    </div>
  );
}

export default LoginPage;
