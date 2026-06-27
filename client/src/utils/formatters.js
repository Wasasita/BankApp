/**
 * Formatting utilities
 */

export function formatCurrency(amount) {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(amount);
}

export function formatDate(date) {
  const d = new Date(date);
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  }).format(d);
}

export function formatDateTime(date) {
  const d = new Date(date);
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(d);
}

export function formatTime(date) {
  const d = new Date(date);
  return new Intl.DateTimeFormat('en-US', {
    hour: '2-digit',
    minute: '2-digit',
  }).format(d);
}

export function getTransactionTypeColor(type) {
  const colors = {
    'Deposit': 'bg-success-light text-success',
    'Withdraw': 'bg-danger-light text-danger',
    'Transfer': 'bg-blue-100 text-primary',
  };
  return colors[type] || 'bg-neutral-100 text-neutral-700';
}

export function getTransactionTypeEmoji(type) {
  const emojis = {
    'Deposit': '💰',
    'Withdraw': '💸',
    'Transfer': '🔄',
  };
  return emojis[type] || '📝';
}
