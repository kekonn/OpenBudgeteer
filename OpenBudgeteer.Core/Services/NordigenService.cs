using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using OpenBudgeteer.Contracts.Enums;
using OpenBudgeteer.Contracts.Models;
using OpenBudgeteer.Core.Common;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Agreements;
using VMelnalksnis.NordigenDotNet.Institutions;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace OpenBudgeteer.Core.Services;

public class NordigenService : IBankConnectionService
{
    private const string BankListCacheKey = "Nordigen_BankList";
    private const string AccountListCacheKeyPrefix = "Nordigen_AccountList";
    private static string AccountListCacheKey(Guid connectionId) => $"{AccountListCacheKeyPrefix}_{connectionId:N}";
    
    private readonly INordigenClient _nordigenClient;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// The services' display name, for use in the import window
    /// </summary>
    public string DisplayName => "Nordigen PSD2 Gateway";
    
    public NordigenService(INordigenClient nordigenClient, IMemoryCache cache)
    {
        _nordigenClient = nordigenClient ?? throw new ArgumentNullException(nameof(nordigenClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Gets a list of banks supported by this client.
    /// </summary>
    /// <param name="country">A two letter country code (ISO 3166)</param>
    public Task<ICollection<Bank>> GetSupportedBanksAsync(string country = null, CancellationToken cancellationToken = default)
    {
        return GetBanksAsyncImpl(country, cancellationToken);
    }

    private async Task<ICollection<Bank>> GetBanksAsyncImpl(string country, CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(BankListCacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromDays(1))
                .SetAbsoluteExpiration(TimeSpan.FromDays(21));
            
            var institutions = await _nordigenClient.Institutions.GetByCountry(country, cancellationToken);

            return institutions.Select(ToBank).ToArray();
        });
    }

    /// <summary>
    /// Get a list of existing banking connections.
    /// </summary>
    /// <returns>A list of existing banking connections</returns>
    public async Task<ICollection<BankConnection>> GetExistingBankConnectionsAsync(
        CancellationToken cancellationToken = default)
    {
        var agreements = _nordigenClient.Agreements.Get(cancellationToken: cancellationToken);

        var result = new List<BankConnection>();
        await foreach (var agreement in agreements.WithCancellation(cancellationToken))    
        {
            result.Add(ToBankConnection(agreement));
        }

        return result;
    }

    /// <summary>
    /// Create a new bank connection.
    /// </summary>
    /// <param name="bankId">The unique bank id</param>
    /// <returns>The new connection</returns>
    /// <exception cref="ArgumentException"></exception>
    public Task<BankConnection> CreateConnectionAsync(string bankId)
    {
        if (string.IsNullOrWhiteSpace(bankId))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(bankId));
        }

        return CreateConnectionAsyncImpl(bankId);
    }

    private async Task<BankConnection> CreateConnectionAsyncImpl(string bankId, 
        BankConnectionAccessScope scope = BankConnectionAccessScope.All,
        (int MaxHistoricalDays, int AccessValidForDays)? expirations = null)
    {
        var allDefaults = !expirations.HasValue && scope == BankConnectionAccessScope.All;

        var newAgreement = allDefaults
            ? new EndUserAgreementCreation(bankId)
            : new EndUserAgreementCreation(bankId, expirations.Value.MaxHistoricalDays,
                expirations.Value.AccessValidForDays, scope.ToStringList());

        var createdAgreement = await _nordigenClient.Agreements.Post(newAgreement);

        return ToBankConnection(createdAgreement);
    }
    
    /// <summary>
    /// Request a link from the bank service we can redirect the user to, so they can authorize the connection.
    /// </summary>
    /// <param name="connection">The previously created bank connection</param>
    /// <param name="redirectUri">The uri that the service should redirect to after the authorization</param>
    /// <param name="reference">An optional reference we can pass in</param>
    /// <returns>A link the user should visit in their browser, to authorize the connection to their bank and a unique id that references this requisition.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any of the required arguments are null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="connection"/>.BankId is null, empty or whitespace.</exception>
    public Task<(Uri, Guid)> CreateConnectionAcceptanceUrlAsync(BankConnection connection, Uri redirectUri, string reference = null)
    {
        if (connection == null) throw new ArgumentNullException(nameof(connection));
        if (redirectUri == null) throw new ArgumentNullException(nameof(redirectUri));

        if (string.IsNullOrWhiteSpace(connection.BankId))
            throw new ArgumentException("The connection should be linked to a bank.", nameof(connection));

        return CreateConnectionAcceptUriImpl(connection, redirectUri, reference);
    }

    /// <summary>
    /// Gets a list of all accounts available under the given connection.
    /// </summary>
    /// <param name="bankConnectionId">The id of the bank connection</param>
    /// <returns>A object containing a list of accounts in the given connection, as wel as it's status.</returns>
    public async Task<BankAccountsInBankConnection> GetBankAccountsAsync(Guid bankConnectionId)
    {
        return ToAccountsList(await _cache.GetOrCreateAsync(AccountListCacheKey(bankConnectionId), 
            async entry => await _nordigenClient.Requisitions.Get(bankConnectionId)));
    }

    private async Task<(Uri, Guid)> CreateConnectionAcceptUriImpl(BankConnection connection, Uri redirectUri, string reference = null)
    {
        var requisition = new RequisitionCreation(redirectUri, connection.BankId)
        {
            Reference = reference
        };

        var uriResponse = await _nordigenClient.Requisitions.Post(requisition);

        return (uriResponse.Link, uriResponse.Id);
    }

    #region Convertors

    private static Bank ToBank(Institution institution)
    {
        return new Bank
        {
            Bic = institution.Bic,
            Id = institution.Id,
            Logo = institution.Logo.ToString(),
            Name = institution.Name,
            TransactionTotalDays = institution.TransactionTotalDays,
            Countries = institution.Countries
        };
    }

    private static BankConnection ToBankConnection(EndUserAgreement agreement)
    {
        return new BankConnection
        {
            Accepted = agreement.Accepted?.ToDateTimeUtc(),
            Created = agreement.Created.ToDateTimeUtc(),
            Id = agreement.Id,
            BankId = agreement.InstitutionId,
            MaxHistoricalDays = agreement.MaxHistoricalDays,
            AccessValidForDays = agreement.AccessValidForDays,
            Scopes = agreement.AccessScope.ToAccessScope()
        };
    }

    private static BankAccountsInBankConnection ToAccountsList(Requisition requisition)
    {
        return new BankAccountsInBankConnection
        {
            Id = requisition.Id,
            Accounts = requisition.Accounts,
            Reference = requisition.Reference,
            Status = Enum.GetName(requisition.Status)
        };
    }
    #endregion
}