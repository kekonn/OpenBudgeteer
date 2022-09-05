using System;
using System.Collections.Generic;

namespace OpenBudgeteer.Core.Models;

public class BankAccountsInBankConnection
{
    public string Status { get; set; }
    public string Reference { get; set; }
    public Guid Id { get; set; }
    public ICollection<Guid> Accounts { get; set; }
}