using System;
using OpenBudgeteer.Contracts.Enums;

namespace OpenBudgeteer.Contracts.Models;

public class BankConnection
{
    /// <summary>
    /// The connection's unique id
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// The moment the connection was created
    /// </summary>
    public DateTime Created { get; set; }
    /// <summary>
    /// The maximum amount of transaction history available in this connection
    /// </summary>
    public int MaxHistoricalDays { get; set; }
    /// <summary>
    /// The amount of days this connection is valid since the date of acceptance
    /// </summary>
    /// <seealso cref="Accepted"/>
    public int AccessValidForDays { get; set; }
    /// <summary>
    /// The moment this connection was accepted
    /// </summary>
    public DateTime? Accepted { get; set; }
    /// <summary>
    /// Set of scopes for this connection
    /// </summary>
    public BankConnectionAccessScope Scopes { get; set; }
    /// <summary>
    /// Id of the bank this connection relates to
    /// </summary>
    public string BankId { get; set; }
}