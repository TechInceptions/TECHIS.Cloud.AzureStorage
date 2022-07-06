using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Text;

namespace TECHIS.Cloud.AzureStorage
{
    public interface ISecretStore
    {
        string GetSecret (string key);
    }

    public interface ISecretStoreFactory
    {
        ISecretStore GetSecretStore ();
    }
    public class DefaultSecretStoreFactory : ISecretStoreFactory
    {
        public ISecretStore GetSecretStore()
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultSecretStore : ISecretStore
    {
        private SecretClient _client;
        public string VaultUri { get; }
        public SecretClientOptions Options { get; }

        //"https://<your-unique-key-vault-name>.vault.azure.net/"
        public DefaultSecretStore(string vaultUri)
        {
            if (string.IsNullOrWhiteSpace(vaultUri))
            {
                throw new ArgumentException($"'{nameof(vaultUri)}' cannot be null or whitespace.", nameof(vaultUri));
            }

            VaultUri = vaultUri;

            Options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = Azure.Core.RetryMode.Exponential
                 }
            };

            _client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential(), Options);
        }


        public string GetSecret(string key)
        {
            KeyVaultSecret secret = _client.GetSecret(key);

            return secret?.Value;

        }
    }
}
