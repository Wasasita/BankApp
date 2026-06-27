/**
 * Transaction data model
 */
export class Transaction {
  constructor(id, accountId, type, amount, description, date, balance) {
    this.id = id;
    this.accountId = accountId;
    this.type = type; // 'Deposit', 'Withdraw', 'Transfer'
    this.amount = amount;
    this.description = description;
    this.date = new Date(date);
    this.balance = balance;
  }

  /**
   * Factory method to create Transaction from API response
   */
  static from(dto) {
    return new Transaction(
      dto.id,
      dto.accountId,
      dto.type,
      dto.amount,
      dto.description,
      dto.date ?? dto.createdAt,
      dto.balance
    );
  }
}

export default Transaction;
