using System.Threading.Tasks;
using NReader.Storage.Abstractions;

namespace NReader.Blazor;

public class Initialisation
{
    private readonly IStorageProvider _storageProvider;

    public Initialisation(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task InitialiseAsync()
    {
        await _storageProvider.InitialiseAsync();
    }
}
