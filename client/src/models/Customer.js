function computeBalanceFromAccounts(accounts) {
  if (!Array.isArray(accounts)) return 0
  return accounts.reduce((sum, account) => sum + (account.balance || 0), 0)
}

export default class Customer {
  constructor({
    id = '',
    name = '',
    email = '',
    totalBalance = 0,
    accountsCount = 0,
    accounts = [],
  } = {}) {
    this.id = id
    this.name = name
    this.email = email
    this.totalBalance = totalBalance
    this.accountsCount = accountsCount
    this.accounts = accounts
  }

  static from(obj) {
    const accounts = Array.isArray(obj.accounts) ? obj.accounts : []
    const totalBalance =
      obj.totalBalance ??
      obj.TotalBalance ??
      computeBalanceFromAccounts(accounts)

    return new Customer({
      id: obj.id,
      name: obj.name,
      email: obj.email,
      totalBalance,
      accountsCount: accounts.length,
      accounts,
    })
  }
}