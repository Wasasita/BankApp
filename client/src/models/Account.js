export default class Account {
  constructor({
    id = '',
    customerId = '',
    accountNumber = '',
    accountType = '',
    balance = 0,
    customerName = '',
  } = {}) {
    this.id = id
    this.customerId = customerId
    this.accountNumber = accountNumber
    this.accountType = accountType
    this.balance = balance
    this.customerName = customerName
  }

  static from(obj) {
    return new Account({
      id: obj.id,
      customerId: obj.customerId,
      accountNumber: obj.accountNumber,
      accountType: obj.accountType,
      balance: obj.balance,
      customerName: obj.customerName || '',
    })
  }
}