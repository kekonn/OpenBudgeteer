using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using OpenBudgeteer.Core.Common;
using OpenBudgeteer.Core.Enums;
using OpenBudgeteer.Core.Models;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Agreements;
using VMelnalksnis.NordigenDotNet.Institutions;

namespace OpenBudgeteer.Blazor.Services;

public class NordigenService : IBankConnectionService
{
    private readonly INordigenClient _nordigenClient;

    public NordigenService(INordigenClient nordigenClient)
    {
        _nordigenClient = nordigenClient ?? throw new ArgumentNullException(nameof(nordigenClient));
    }

    /// <summary>
    /// Gets a list of banks supported by this client.
    /// </summary>
    /// <param name="country">A two letter country code (ISO 3166)</param>
    public Task<ICollection<Bank>> GetSupportedBanksAsync(string country, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(country));
        }

        return GetBanksAsyncImpl(country, cancellationToken);
    }

    private async Task<ICollection<Bank>> GetBanksAsyncImpl(string country, CancellationToken cancellationToken = default)
    {
        var institutions = await _nordigenClient.Institutions.GetByCountry(country, cancellationToken);

        return institutions.Select(ToBank).ToArray();
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
    #endregion
}