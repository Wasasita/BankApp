export default class Account {
  constructor({
    id = '',
    accountNumber = '',
    accountType = '',
    balance = 0
  } = {}) {
    this.id = id
    this.accountNumber = accountNumber
    this.accountType = accountType
    this.balance = balance
  }

  static from(obj) {
    return new Account({
      id: obj.id,
      accountNumber: obj.accountNumber,
      accountType: obj.accountType,
      balance: obj.balance
    })
  }
}