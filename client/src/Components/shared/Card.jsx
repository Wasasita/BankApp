/**
 * EmptyState component - Display when no data available
 */
export function EmptyState({ title, description, action }) {
  return (
    <div className="flex flex-col items-center justify-center py-12 px-4 text-center">
      <div className="text-5xl mb-4">{icon}</div>
      <h3 className="text-lg font-semibold text-neutral-900 mb-2">{title}</h3>
      <p className="text-neutral-600 mb-6 max-w-sm">{description}</p>
      {action && <div>{action}</div>}
    </div>
  );
}

/**
 * Card component - Reusable card wrapper
 */
export function Card({ children, className = '', header, footer }) {
  return (
    <div className={`bg-white rounded-lg shadow-sm border border-neutral-200 overflow-hidden ${className}`}>
      {header && <div className="border-b border-neutral-200 px-6 py-4">{header}</div>}
      <div className="p-6">{children}</div>
      {footer && <div className="border-t border-neutral-200 px-6 py-4 bg-neutral-50">{footer}</div>}
    </div>
  );
}
