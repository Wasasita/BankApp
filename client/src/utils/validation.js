import { z } from 'zod';

/**
 * Validation schemas using Zod
 */

export const loginSchema = z.object({
  email: z
    .string()
    .email('Invalid email address')
    .min(1, 'Email is required'),
  password: z
    .string()
    .min(1, 'Password is required')
    .min(6, 'Password must be at least 6 characters'),
});

export const customerSchema = z.object({
  name: z
    .string()
    .min(1, 'Name is required')
    .min(2, 'Name must be at least 2 characters'),
  email: z
    .string()
    .email('Invalid email address')
    .min(1, 'Email is required'),
});

export const accountSchema = z.object({
  accountNumber: z
    .string()
    .min(1, 'Account number is required'),
  accountType: z
    .string()
    .min(1, 'Account type is required'),
  balance: z
    .number()
    .min(0, 'Balance cannot be negative'),
  customerId: z
    .number()
    .positive('Customer ID must be positive'),
});

export const depositSchema = z.object({
  accountId: z
    .number()
    .positive('Account is required'),
  amount: z
    .number()
    .positive('Amount must be greater than 0'),
  description: z
    .string()
    .optional()
    .default(''),
});

export const withdrawSchema = z.object({
  accountId: z
    .number()
    .positive('Account is required'),
  amount: z
    .number()
    .positive('Amount must be greater than 0'),
  description: z
    .string()
    .optional()
    .default(''),
});

export const transferSchema = z.object({
  fromAccountId: z
    .number()
    .positive('From account is required'),
  toAccountId: z
    .number()
    .positive('To account is required'),
  amount: z
    .number()
    .positive('Amount must be greater than 0'),
  description: z
    .string()
    .optional()
    .default(''),
}).refine(
  (data) => data.fromAccountId !== data.toAccountId,
  {
    message: 'Source and destination accounts must be different',
    path: ['toAccountId'],
  }
);
