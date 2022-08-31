using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenBudgeteer.Core.Models;

namespace OpenBudgeteer.Blazor.Services;

public interface IBankConnectionService
{
    /// <summary>
    /// Gets a list of banks supported by this client.
    /// </summary>
    /// <param name="country">A two letter country code (ISO 3166)</param>
    Task<ICollection<Bank>> GetSupportedBanksAsync(string country, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a list of existing banking connections.
    /// </summary>
    /// <returns>A list of existing banking connections</returns>
    Task<ICollection<BankConnection>> GetExistingBankConnectionsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new bank connection.
    /// </summary>
    /// <param name="bankId">The unique bank id</param>
    /// <returns>The new connection</returns>
    /// <exception cref="ArgumentException"></exception>
    Task<BankConnection> CreateConnectionAsync(string bankId);
}