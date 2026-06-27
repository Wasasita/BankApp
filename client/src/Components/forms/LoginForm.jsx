import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginSchema } from '../../utils/validation';
import { ErrorAlert } from '../shared/ErrorAlert';

/**
 * LoginForm component
 */
export function LoginForm({ onSubmit, isLoading = false }) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: zodResolver(loginSchema),
    mode: 'onBlur',
  });

  const [error, setError] = useState(null);

  const onSubmitHandler = async (data) => {
    setError(null);
    try {
      await onSubmit(data);
    } catch (err) {
      setError(err.message);
    }
  };

  const isLoadingState = isLoading || isSubmitting;

  return (
    <form onSubmit={handleSubmit(onSubmitHandler)} className="space-y-4">
      {error && (
        <ErrorAlert
          message={error}
          onClose={() => setError(null)}
        />
      )}

      {/* Email */}
      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Email Address
        </label>
        <input
          type="email"
          placeholder="you@example.com"
          className="input-field"
          disabled={isLoadingState}
          {...register('email')}
        />
        {errors.email && (
          <p className="mt-1 text-sm text-danger">{errors.email.message}</p>
        )}
      </div>

      {/* Password */}
      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Password
        </label>
        <input
          type="password"
          placeholder="••••••••"
          className="input-field"
          disabled={isLoadingState}
          {...register('password')}
        />
        {errors.password && (
          <p className="mt-1 text-sm text-danger">{errors.password.message}</p>
        )}
      </div>

      {/* Submit Button */}
      <button
        type="submit"
        disabled={isLoadingState}
        className="btn-primary w-full disabled:opacity-50"
      >
        {isLoadingState ? 'Signing in...' : 'Sign In'}
      </button>

      {/* Demo Credentials Info */}
      <div className="p-3 bg-blue-50 border border-blue-200 rounded-lg text-sm text-neutral-700">
        <p className="font-medium mb-1">Demo Credentials:</p>
        <p>Email: demo@example.com</p>
        <p>Password: password</p>
      </div>
    </form>
  );
}
