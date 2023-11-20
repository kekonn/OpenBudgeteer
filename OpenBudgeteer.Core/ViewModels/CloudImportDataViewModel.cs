using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenBudgeteer.Core.Common;
using TinyCsvParser.Mapping;
using System.Threading;
using OpenBudgeteer.Contracts.Models;
using OpenBudgeteer.Core.Services;
using OpenBudgeteer.Core.ViewModels.ItemViewModels;
using OpenBudgeteer.Data;

namespace OpenBudgeteer.Core.ViewModels;

public class CloudImportDataViewModel : ViewModelBase
{
    private ObservableCollection<BankViewModel> _banks;
    public ObservableCollection<BankViewModel> Banks
    {
        get => _banks;
        private set => Set(ref _banks, value);
    }

    private BankViewModel _selectedBank;
    public BankViewModel SelectedBank
    {
        get => _selectedBank;
        set => Set(ref _selectedBank, value);
    }

    private ObservableCollection<BankServiceViewModelItem> _bankServiceViewModels;
    public ObservableCollection<BankServiceViewModelItem> BankServices
    {
        get => _bankServiceViewModels;
        private set => Set(ref _bankServiceViewModels, value);
    }

    private Account _selectedAccount;
    /// <summary>
    /// Target <see cref="Account"/> for which all imported <see cref="BankTransaction"/> should be added
    /// </summary>
    public Account SelectedAccount
    {
        get => _selectedAccount;
        set => Set(ref _selectedAccount, value);
    }

    private int _totalRecords;
    /// <summary>
    /// Number of records identified in the file
    /// </summary>
    public int TotalRecords
    {
        get => _totalRecords;
        private set => Set(ref _totalRecords, value);
    }

    private int _recordsWithErrors;
    /// <summary>
    /// Number of records where import will fail or has failed
    /// </summary>
    public int RecordsWithErrors
    {
        get => _recordsWithErrors;
        private set => Set(ref _recordsWithErrors, value);
    }

    private int _validRecords;
    /// <summary>
    /// Number of records where import will be or was successful
    /// </summary>
    public int ValidRecords
    {
        get => _validRecords;
        private set => Set(ref _validRecords, value);
    }
    
    private int _potentialDuplicates;
    /// <summary>
    /// Number of records which have been identified as potential duplicate
    /// </summary>
    public int PotentialDuplicates
    {
        get => _potentialDuplicates;
        private set => Set(ref _potentialDuplicates, value);
    }

    private ObservableCollection<Account> _availableAccounts;
    /// <summary>
    /// Helper collection to list all available <see cref="Account"/> in the database
    /// </summary>
    public ObservableCollection<Account> AvailableAccounts
    {
        get => _availableAccounts;
        private set => Set(ref _availableAccounts, value);
    }

    private ObservableCollection<CsvMappingResult<BankTransaction>> _parsedRecords;
    /// <summary>
    /// Results of <see cref="TinyCsvParser"/>
    /// </summary>
    public ObservableCollection<CsvMappingResult<BankTransaction>> ParsedRecords
    {
        get => _parsedRecords;
        private set => Set(ref _parsedRecords, value);
    }

    private bool _isProfileValid;
    private readonly DbContextOptions<DatabaseContext> _dbOptions;
    private readonly IReadOnlyCollection<IBankConnectionService> _bankServices;

    /// <summary>
    /// Basic constructor
    /// </summary>
    /// <param name="dbOptions">Options to connect to a database</param>
    public CloudImportDataViewModel(DbContextOptions<DatabaseContext> dbOptions, IEnumerable<IBankConnectionService> bankServices)
    {
        AvailableAccounts = new ObservableCollection<Account>();
        ParsedRecords = new ObservableCollection<CsvMappingResult<BankTransaction>>();
        SelectedAccount = new Account();
        Banks = new ObservableCollection<BankViewModel>();
        _dbOptions = dbOptions;
        _bankServices = bankServices?.ToArray() ?? throw new ArgumentNullException(nameof(bankServices));
    }

    /// <summary>
    /// Initialize ViewModel and load data from database
    /// </summary>
    /// <returns></returns>
    public async Task<ViewModelOperationResult> LoadDataAsync()
    {
        try
        {
            await using (var dbContext = new DatabaseContext(_dbOptions))
            {
                foreach (var account in dbContext.Account.Where(i => i.IsActive == 1))
                {
                    AvailableAccounts.Add(account);
                }
            }

            var banks = (await Task.WhenAll(
                _bankServices.Select(b => b.GetSupportedBanksAsync()))
                ).SelectMany(e => e)
                .Select(b => new BankViewModel
                {
                    Id = b.Id,
                    LogoUrl = b.Logo,
                    Name = b.Name
                });
            Banks = new ObservableCollection<BankViewModel>(banks);

            BankServices = new ObservableCollection<BankServiceViewModelItem>(
                _bankServices.Select(b => new BankServiceViewModelItem
                {
                    Id = b.GetType().FullName,
                    Name = b.DisplayName
                })
            );
            
            return new ViewModelOperationResult(true);
        }
        catch (Exception e)
        {
            return new ViewModelOperationResult(false, $"Error during loading: {e.Message}");
        }           
    }

    /// <summary>
    /// Reads the file and parses the content to a set of <see cref="BankTransaction"/>.
    /// Results will be stored in <see cref="ParsedRecords"/>
    /// </summary>
    /// <remarks>
    /// Sets also figures of the ViewModel like <see cref="TotalRecords"/> or <see cref="ValidRecords"/>
    /// </remarks>
    /// <returns>Object which contains information and results of this method</returns>
    public Task<ViewModelOperationResult> ValidateDataAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uses data from <see cref="ParsedRecords"/> to store it in the database
    /// </summary>
    /// <returns>Object which contains information and results of this method</returns>
    public async Task<ViewModelOperationResult> ImportDataAsync(CancellationToken cancellationToken = default)
    {
        if (!_isProfileValid) return new ViewModelOperationResult(false, "Unable to Import Data as current settings are invalid.");

        await using var dbContext = new DatabaseContext(_dbOptions);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var importedCount = 0;
            var newRecords = new List<BankTransaction>();
            foreach (var parsedRecord in ParsedRecords.Where(i => i.IsValid))
            {
                var newRecord = parsedRecord.Result;
                newRecord.AccountId = SelectedAccount.AccountId;
                newRecords.Add(newRecord);
            }
            importedCount = dbContext.CreateBankTransactions(newRecords);
                    
            await transaction.CommitAsync(cancellationToken);
            return new ViewModelOperationResult(true, $"Successfully imported {importedCount} records.");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new ViewModelOperationResult(false, $"Unable to Import Data. Error message: {e.Message}");
        }
    }

    /// <summary>
    /// Looks up the <see cref="IBankConnectionService"/> for the given <see cref="BankServiceViewModelItem"/>.
    /// </summary>
    /// <param name="selectedBankService">The <see cref="BankServiceViewModelItem"/> to look up.</param>
    /// <returns>The first known bank service that matches the selected bank service by id/typename, or null if none was found.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="selectedBankService"/> is null.</exception>
    public IBankConnectionService GetSelectedBankConnection(BankServiceViewModelItem selectedBankService)
    {
        if (selectedBankService == null) throw new ArgumentNullException(nameof(selectedBankService));

        return _bankServices.FirstOrDefault(b => b.GetType().FullName.Equals(selectedBankService.Id));
    }
}
