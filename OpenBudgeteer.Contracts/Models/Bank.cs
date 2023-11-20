using System.Collections.Generic;

namespace OpenBudgeteer.Contracts.Models;

public class Bank
{
    /// <summary>
    /// Unique Bank ID
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Bank's user friendly name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Bank Identification Code.
    /// </summary>
    public string Bic { get; set; }
    /// <summary>
    /// The amount of days of transaction history that's available
    /// </summary>
    public int TransactionTotalDays { get; set; }
    /// <summary>
    /// The countries the bank operates in
    /// </summary>
    public ICollection<string> Countries { get; set; }
    /// <summary>
    /// A URL to the bank's logo
    /// </summary>
    public string Logo { get; set; }
}