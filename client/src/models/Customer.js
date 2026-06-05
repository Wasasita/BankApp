export default class Customer {
  constructor({
    id = '',
    name = '',
    email = '',
    totalBalance = 0
  } = {}) {
    this.id = id
    this.name = name
    this.email = email
    this.totalBalance = totalBalance
  }

  static from(obj) {
    return new Customer({
      id: obj.id,
      name: obj.name,
      email: obj.email,
      totalBalance: obj.totalBalance ?? obj.totalBalance === 0 ? 0 : 0
    })
  }
}