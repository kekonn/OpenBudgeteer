using System;

namespace OpenBudgeteer.Core.ViewModels;

public class BankConnectionViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string BankIconUrl { get; set; }
}