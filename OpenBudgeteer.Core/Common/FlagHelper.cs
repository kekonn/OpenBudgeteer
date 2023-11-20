using System;
using System.Collections.Generic;
using System.Linq;
using OpenBudgeteer.Contracts.Enums;

namespace OpenBudgeteer.Core.Common;

public static class FlagHelper
{
    public static BankConnectionAccessScope ToAccessScope(this ICollection<string> scopes,
        BankConnectionAccessScope defaultScope = BankConnectionAccessScope.All)
    {
        if (scopes is null or { Count: 0 })
        {
            return defaultScope;
        }

        var scope = BankConnectionAccessScope.None;
        foreach (var scopeDesc in scopes.Distinct())
        {
            var parsedScope = Enum.Parse<BankConnectionAccessScope>(scopeDesc);
            if (scope.HasFlag(parsedScope))
            {
                scope |= parsedScope;
            }
        }

        return scope;
    }

    public static List<string> ToStringList(this BankConnectionAccessScope scope)
    {
        if (scope.HasFlag(BankConnectionAccessScope.All))
        {
            return new List<string>
            {
                BankConnectionAccessScope.Balances.ToString()?.ToLowerInvariant(),
                BankConnectionAccessScope.Details.ToString()?.ToLowerInvariant(),
                BankConnectionAccessScope.Transactions.ToString()?.ToLowerInvariant()
            };
        }

        return Enum.GetNames<BankConnectionAccessScope>()
            .Select(Enum.Parse<BankConnectionAccessScope>)
            .Where(e => scope.HasFlag(e))
            .Select(e => e.ToString().ToLowerInvariant())
            .ToList();
    }
}