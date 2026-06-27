import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import DataService from './DataService';
import Customer from '../models/Customer';
import Account from '../models/Account';
import Transaction from '../models/Transaction';

// Query Keys
const QUERY_KEYS = {
  customers: ['customers'],
  customer: (id) => ['customer', id],
  searchCustomers: (name) => ['customers', 'search', name],
  searchCustomersByEmail: (email) => ['customers', 'search-email', email],
  premiumCustomers: (threshold) => ['customers', 'premium', threshold],
  customerBalance: (id) => ['customer', id, 'balance'],
  accounts: ['accounts'],
  account: (id) => ['account', id],
  searchAccounts: (name) => ['accounts', 'search', name],
  customerAccounts: (customerId) => ['accounts', 'customer', customerId],
  transactions: ['transactions'],
  transaction: (id) => ['transaction', id],
  accountTransactions: (accountId) => ['transactions', 'account', accountId],
};

// ============ CUSTOMERS QUERIES ============

export function useCustomers() {
  return useQuery({
    queryKey: QUERY_KEYS.customers,
    queryFn: () => DataService.getCustomers().then(data => 
      Array.isArray(data) ? data.map(Customer.from) : []
    ),
  });
}

export function useCustomer(id) {
  return useQuery({
    queryKey: QUERY_KEYS.customer(id),
    queryFn: () => DataService.getCustomer(id).then(Customer.from),
    enabled: !!id,
  });
}

export function useSearchCustomers(name) {
  return useQuery({
    queryKey: QUERY_KEYS.searchCustomers(name),
    queryFn: () => DataService.searchCustomers(name).then(data =>
      Array.isArray(data) ? data.map(Customer.from) : []
    ),
    enabled: !!name && name.trim().length > 0,
  });
}

export function useSearchCustomersByEmail(email) {
  return useQuery({
    queryKey: QUERY_KEYS.searchCustomersByEmail(email),
    queryFn: () => DataService.searchCustomersByEmail(email).then(data =>
      Array.isArray(data) ? data.map(Customer.from) : []
    ),
    enabled: !!email && email.trim().length > 0,
  });
}

export function usePremiumCustomers(threshold = 0, enabled = true) {
  return useQuery({
    queryKey: QUERY_KEYS.premiumCustomers(threshold),
    queryFn: () => DataService.getPremiumCustomers(threshold).then(data =>
      Array.isArray(data) ? data.map(Customer.from) : []
    ),
    enabled,
  });
}

export function useCustomerBalance(id) {
  return useQuery({
    queryKey: QUERY_KEYS.customerBalance(id),
    queryFn: () => DataService.getCustomerTotalBalance(id),
    enabled: !!id,
  });
}

export function useCreateCustomer() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (customer) => DataService.createCustomer(customer),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.customers });
      queryClient.invalidateQueries({ queryKey: ['customers', 'premium'] });
    },
  });
}

export function useUpdateCustomer() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, customer }) => DataService.updateCustomer(id, customer),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.customers });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.customer(id) });
      queryClient.invalidateQueries({ queryKey: ['customers', 'premium'] });
    },
  });
}

export function useDeleteCustomer() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id) => DataService.deleteCustomer(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.customers });
      queryClient.invalidateQueries({ queryKey: ['customers', 'premium'] });
    },
  });
}

// ============ ACCOUNTS QUERIES ============

export function useAccounts() {
  return useQuery({
    queryKey: QUERY_KEYS.accounts,
    queryFn: () => DataService.getAccounts().then(data =>
      Array.isArray(data) ? data.map(Account.from) : []
    ),
  });
}

export function useAccount(id) {
  return useQuery({
    queryKey: QUERY_KEYS.account(id),
    queryFn: () => DataService.getAccount(id).then(Account.from),
    enabled: !!id,
  });
}

export function useSearchAccounts(name) {
  return useQuery({
    queryKey: QUERY_KEYS.searchAccounts(name),
    queryFn: () => DataService.searchAccounts(name).then(data =>
      Array.isArray(data) ? data.map(Account.from) : []
    ),
    enabled: !!name && name.length > 0,
  });
}

export function useAccountsByCustomer(customerId) {
  return useQuery({
    queryKey: QUERY_KEYS.customerAccounts(customerId),
    queryFn: () => DataService.getAccountsByCustomerId(customerId).then(data =>
      Array.isArray(data) ? data.map(Account.from) : []
    ),
    enabled: !!customerId,
  });
}

export function useCreateAccount() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ account, customerId }) => 
      DataService.createAccount(account, customerId),
    onSuccess: (_, { customerId }) => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accounts });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.customerAccounts(customerId) });
    },
  });
}

export function useUpdateAccount() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, account }) => DataService.updateAccount(id, account),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accounts });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.account(id) });
    },
  });
}

export function useDeleteAccount() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id) => DataService.deleteAccount(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accounts });
    },
  });
}

// ============ TRANSACTIONS QUERIES ============

export function useTransactions() {
  return useQuery({
    queryKey: QUERY_KEYS.transactions,
    queryFn: () => DataService.getTransactions().then(data =>
      Array.isArray(data) ? data.map(Transaction.from) : []
    ),
  });
}

export function useTransaction(id) {
  return useQuery({
    queryKey: QUERY_KEYS.transaction(id),
    queryFn: () => DataService.getTransaction(id).then(Transaction.from),
    enabled: !!id,
  });
}

export function useAccountTransactions(accountId) {
  return useQuery({
    queryKey: QUERY_KEYS.accountTransactions(accountId),
    queryFn: () => DataService.getAccountTransactions(accountId).then(data =>
      Array.isArray(data) ? data.map(Transaction.from) : []
    ),
    enabled: !!accountId,
  });
}

export function useDeposit() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ accountId, amount, description }) =>
      DataService.deposit(accountId, amount, description),
    onSuccess: (_, { accountId }) => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.transactions });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accountTransactions(accountId) });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accounts });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.account(accountId) });
    },
  });
}

export function useWithdraw() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ accountId, amount, description }) =>
      DataService.withdraw(accountId, amount, description),
    onSuccess: (_, { accountId }) => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.transactions });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accountTransactions(accountId) });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accounts });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.account(accountId) });
    },
  });
}

export function useTransfer() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ fromAccountId, toAccountId, amount, description }) =>
      DataService.transfer(fromAccountId, toAccountId, amount, description),
    onSuccess: (_, { fromAccountId, toAccountId }) => {
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.transactions });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accountTransactions(fromAccountId) });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accountTransactions(toAccountId) });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.accounts });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.account(fromAccountId) });
      queryClient.invalidateQueries({ queryKey: QUERY_KEYS.account(toAccountId) });
    },
  });
}
