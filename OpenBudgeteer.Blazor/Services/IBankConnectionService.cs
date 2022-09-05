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

    /// <summary>
    /// Request a link from the bank service we can redirect the user to, so they can authorize the connection.
    /// </summary>
    /// <param name="connection">The previously created bank connection</param>
    /// <param name="redirectUri">The uri that the service should redirect to after the authorization</param>
    /// <param name="reference">An optional reference we can pass in</param>
    /// <returns>A link the user should visit in their browser, to authorize the connection to their bank and a unique id that references this requisition.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any of the required arguments are null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="connection"/>.BankId is null, empty or whitespace.</exception>
    Task<(Uri, Guid)> CreateConnectionAcceptanceUrlAsync(BankConnection connection, Uri redirectUri, string reference = null);

    /// <summary>
    /// Gets a list of all accounts available under the given connection.
    /// </summary>
    /// <param name="bankConnectionId">The id of the bank connection</param>
    /// <returns>A object containing a list of accounts in the given connection, as wel as it's status.</returns>
    Task<BankAccountsInBankConnection> GetBankAccountsAsync(Guid bankConnectionId);
}