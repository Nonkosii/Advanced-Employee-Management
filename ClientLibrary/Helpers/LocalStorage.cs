

using Blazored.LocalStorage;

namespace ClientLibrary.Helpers
{
    public class LocalStorage(ILocalStorageService localStorage)
    {
        private const string StorageKey = "authentication-token";
        public async Task<string?> GetTokenAsync() => await localStorage.GetItemAsStringAsync(StorageKey);
        public async Task SetTokenAsync(string item) => await localStorage.SetItemAsStringAsync(StorageKey, item);
        public async Task RemoveTokenAsync() => await localStorage.RemoveItemAsync(StorageKey);
    }
}
