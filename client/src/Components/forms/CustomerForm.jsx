import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { customerSchema } from '../../utils/validation';

/**
 * CustomerForm component - Form for creating/editing customers
 */
export function CustomerForm({ onSubmit, onCancel, initialData = null, isLoading = false }) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm({
    resolver: zodResolver(customerSchema),
    mode: 'onBlur',
    defaultValues: initialData || { name: '', email: '' },
  });

  const onSubmitHandler = async (data) => {
    try {
      await onSubmit(data);
      reset();
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  const isSubmittingState = isLoading || isSubmitting;

  return (
    <form onSubmit={handleSubmit(onSubmitHandler)} className="space-y-4">
      {/* Name */}
      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Name
        </label>
        <input
          type="text"
          placeholder="John Doe"
          className="input-field"
          disabled={isSubmittingState}
          {...register('name')}
        />
        {errors.name && (
          <p className="mt-1 text-sm text-danger">{errors.name.message}</p>
        )}
      </div>

      {/* Email */}
      <div>
        <label className="block text-sm font-medium text-neutral-700 mb-2">
          Email
        </label>
        <input
          type="email"
          placeholder="john@example.com"
          className="input-field"
          disabled={isSubmittingState}
          {...register('email')}
        />
        {errors.email && (
          <p className="mt-1 text-sm text-danger">{errors.email.message}</p>
        )}
      </div>

      {/* Buttons */}
      <div className="flex gap-3 justify-end">
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmittingState}
          className="btn-secondary disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isSubmittingState}
          className="btn-primary disabled:opacity-50"
        >
          {isSubmittingState ? 'Saving...' : 'Save'}
        </button>
      </div>
    </form>
  );
}
