﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using NuGet.Services.KeyVault;

namespace NuGet.Services.Configuration
{
    public class ConfigurationRootSecretReaderFactory : ISecretReaderFactory
    {
        private string _vaultName;
        private string _clientId;
        private string _certificateThumbprint;
        private string _storeName;
        private string _storeLocation;
        private bool _validateCertificate;

        public ConfigurationRootSecretReaderFactory(IConfigurationRoot config)
        {
            _vaultName = config[Constants.KeyVaultVaultNameKey];
            _clientId = config[Constants.KeyVaultClientIdKey];
            _certificateThumbprint = config[Constants.KeyVaultCertificateThumbprintKey];
            _storeName = config[Constants.KeyVaultStoreNameKey];
            _storeLocation = config[Constants.KeyVaultStoreLocationKey];
            _validateCertificate = bool.Parse(config[Constants.KeyVaultValidateCertificateKey]);
        }

        public ISecretReader CreateSecretReader()
        {
            if (string.IsNullOrEmpty(_vaultName))
            {
                return new EmptySecretReader();
            }

            var keyVaultConfiguration = new KeyVaultConfiguration(
                _vaultName,
                _clientId,
                _certificateThumbprint,
                !string.IsNullOrEmpty(_storeName) 
                    ? (StoreName) Enum.Parse(typeof(StoreName), _storeName)
                    : StoreName.My,
                !string.IsNullOrEmpty(_storeLocation)
                    ? (StoreLocation) Enum.Parse(typeof(StoreLocation), _storeLocation)
                    : StoreLocation.LocalMachine,
                _validateCertificate);

            return new KeyVaultReader(keyVaultConfiguration);
        }

        public ISecretInjector CreateSecretInjector(ISecretReader secretReader)
        {
            return new SecretInjector(secretReader);
        }
    }
}
