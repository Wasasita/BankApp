export default class Customer {
  constructor({
    id = '',
    name = '',
    email = ''
  } = {}) {
    this.id = id
    this.name = name
    this.email = email
  }

  static from(obj) {
    return new Customer({
      id: obj.id,
      name: obj.name,
      email: obj.email
    })
  }
}