using System;

namespace OpenBudgeteer.Contracts.Enums;

[Flags]
public enum BankConnectionAccessScope : short
{
    None = 0,
    Balances = 1,
    Details = 1 << 2,
    Transactions = 1 << 3,
    All = Balances | Details | Transactions
}