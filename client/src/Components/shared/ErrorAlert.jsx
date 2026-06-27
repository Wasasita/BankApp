import { useState, useEffect } from 'react';

/**
 * ErrorAlert component - Display error messages
 */
export function ErrorAlert({ message, onClose }) {
  return (
    <div className="bg-danger-light border border-danger text-danger px-4 py-3 rounded-lg flex items-start gap-3">
      <span className="text-xl flex-shrink-0">⚠️</span>
      <div className="flex-1">
        <p className="font-medium">{message}</p>
      </div>
      {onClose && (
        <button onClick={onClose} className="text-danger hover:text-red-700">
          ✕
        </button>
      )}
    </div>
  );
}

/**
 * SuccessMessage component - Auto-dismissing success notification
 */
export function SuccessMessage({ message, duration = 3000, onDismiss }) {
  const [isVisible, setIsVisible] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
      setIsVisible(false);
      onDismiss?.();
    }, duration);

    return () => clearTimeout(timer);
  }, [duration, onDismiss]);

  if (!isVisible) return null;

  return (
    <div className="bg-success-light border border-success text-success px-4 py-3 rounded-lg flex items-start gap-3 animate-in fade-in slide-in-from-top-2">
      <span className="text-xl flex-shrink-0">✓</span>
      <div className="flex-1">
        <p className="font-medium">{message}</p>
      </div>
    </div>
  );
}
