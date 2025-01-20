﻿namespace ServerApp.BLL.Services.InterfaceServices
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
