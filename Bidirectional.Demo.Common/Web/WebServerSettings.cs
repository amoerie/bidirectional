using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Bidirectional.Demo.Common.Web
{
    public class WebServerSettings
    {
        public string[]? AllowedHosts { get; set; }

        public HttpSettings? Http { get; set; }
        public HttpsSettings? Https { get; set; }

        public IEnumerable<string> Validate()
        {
            if (AllowedHosts == null || AllowedHosts.Length == 0)
            {
                yield return $"Web server cannot start up because {nameof(WebServerSettings)}.{nameof(AllowedHosts)} is not configured";
            }

            var isHttpEnabled = Http?.IsEnabled == true;
            var isHttpsEnabled = Https?.IsEnabled == true;

            if (!isHttpEnabled && !isHttpsEnabled)
            {
                yield return $"Web server cannot start up because {nameof(WebServerSettings)}.{nameof(Http)} nor {nameof(WebServerSettings)}.{nameof(Https)} is enabled";
            }

            if (isHttpEnabled)
                foreach (var error in Http!.Validate())
                    yield return error;
            if (isHttpsEnabled)
                foreach (var error in Https!.Validate())
                    yield return error;
        }
    }

    internal static class StringExtensions
    {
        public static IPAddress? ToIpAddress(this string? @this)
        {
            if (@this == null)
                return null;

            switch (@this.ToUpper(CultureInfo.InvariantCulture))
            {
                case "ANY":
                case "*":
                    return IPAddress.Any;
                case "LOOPBACK":
                case "LOCALHOST":
                    return IPAddress.Loopback;
                default:
                    return IPAddress.TryParse(@this, out var parsed) ? parsed : null;
            }
        }
    }

    public class HttpSettings
    {
        public bool IsEnabled { get; set; } = true;
        public string? IPAddress { get; set; }
        public int? Port { get; set; }

        public IEnumerable<string> Validate()
        {
            if (!IsEnabled)
            {
                yield break;
            }

            if (IPAddress.ToIpAddress() == null)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Http)}.{nameof(IPAddress)} is not a valid value";
            }

            if (Port == default)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Http)}.{nameof(Port)} is not configured correctly";
            }
        }
    }

    public class HttpsSettings
    {
        public bool IsEnabled { get; set; } = true;
        public string? IPAddress { get; set; }
        public int? Port { get; set; }
        public CertificateSettings? Certificate { get; set; }

        public IEnumerable<string> Validate()
        {
            if (!IsEnabled)
            {
                yield break;
            }

            if (IPAddress.ToIpAddress() == null)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Https)}.{nameof(IPAddress)} is not a valid value";
            }

            if (Port == null || Port == 0)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Https)}.{nameof(Port)} is not configured correctly";
            }

            if (Certificate == null)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Https)}.{nameof(Certificate)} is not configured correctly";
            }
            else
            {
                foreach (var error in Certificate.Validate())
                    yield return error;
            }
        }
    }

    public class CertificateSettings
    {
        public StoreName? StoreName { get; set; }
        public StoreLocation? StoreLocation { get; set; }
        public X509FindType? FindBy { get; set; }
        public string? FindValue { get; set; }

        public IEnumerable<string> Validate()
        {
            if (StoreName == null)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Https)}.{nameof(HttpsSettings.Certificate)}.{nameof(StoreName)} is not configured correctly";
            }

            if (StoreLocation == null)
            {
                yield return
                    $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Https)}.{nameof(HttpsSettings.Certificate)}.{nameof(StoreLocation)} is not configured correctly";
            }

            if (FindBy == null)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Https)}.{nameof(HttpsSettings.Certificate)}.{nameof(FindBy)} is not configured correctly";
            }

            if (FindValue == null)
            {
                yield return $"{nameof(WebServerSettings)}.{nameof(WebServerSettings.Https)}.{nameof(HttpsSettings.Certificate)}.{nameof(FindValue)} is not configured correctly";
            }

            WebServerSettingsException? exception = null;
            X509Certificate2? certificate = null;
            try
            {
                certificate  = FindCertificate();
            }
            catch (WebServerSettingsException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                yield return exception.Message;
            }

            if (certificate != null)
            {
                foreach (var error in ValidateCertificate(certificate))
                {
                    yield return error;
                }
            }
        }

        public X509Certificate2? FindCertificate()
        {
            if (StoreName == null || StoreLocation == null || FindBy == null || FindValue == null)
                return null;

            using (X509Store store = new X509Store(StoreName.Value, StoreLocation.Value))
            {
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                }
                catch (SecurityException e)
                {
                    throw new WebServerSettingsException("Failed to load HTTPS certificate because the application does not have the necessary rights to open the configured certificate store", e);
                }
                catch (CryptographicException e)
                {
                    throw new WebServerSettingsException("Failed to load HTTPS certificate because the certificate store is unreadable", e);
                }
                catch (ArgumentException e)
                {
                    throw new WebServerSettingsException("Failed to load HTTPS certificate because the certificate store contains invalid values", e);
                }

                X509Certificate2Collection collection;

                try
                {
                    collection = store.Certificates.Find(FindBy.Value, FindValue, true);
                }
                catch (CryptographicException e)
                {
                    throw new WebServerSettingsException($"Failed to load HTTPS certificate because the '{nameof(FindBy)}' parameter is invalid", e);
                }

                if (collection.Count == 0)
                {
                    throw new WebServerSettingsException($"There are no certificates in certificate store StoreName='{StoreName}' StoreLocation='{StoreLocation}'" +
                                                           $" where '{FindBy}' = '{FindValue}'");
                }

                if (collection.Count > 1)
                {
                    var certificates = string.Join(", ", collection
                        .Cast<X509Certificate2>()
                        .Select(c => $"Subject = {c.SubjectName}, Thumbprint = {c.Thumbprint}")
                    );
                    throw new WebServerSettingsException($"There are multiple certificates in certificate store StoreName='{StoreName}' StoreLocation='{StoreLocation}' " +
                                                           $"where '{FindBy}' = '{FindValue}': {certificates}");
                }

                return collection[0];
            }
        }

        [SuppressMessage("System", "CA1031")]
        private IEnumerable<string> ValidateCertificate(X509Certificate2 certificate)
        {
            if (!certificate.HasPrivateKey)
            {
                yield return $"Certificate with Subject = {certificate.SubjectName.Name} and Thumbprint = {certificate.Thumbprint} does not have a private key," +
                             $"or the user running this service does not have access to the private key.";
            }

            string? privateKeyError = null;
            try
            {
                var _ = certificate.PrivateKey;
            }
            catch (Exception e)
            {
                privateKeyError = $"Certificate with Subject = {certificate.SubjectName.Name} and Thumbprint = {certificate.Thumbprint} has a private key, but it is not accessible: {e.Message} {e.GetType()} {e.StackTrace}";
            }

            if (privateKeyError != null)
            {
                yield return privateKeyError;
            }
            
            if (DateTime.Now > certificate.NotAfter)
            {
                yield return $"Certificate with Subject = {certificate.SubjectName.Name} and Thumbprint = {certificate.Thumbprint} has expired since {certificate.NotAfter:F}.";
            }

            if (DateTime.Now < certificate.NotBefore)
            {
                yield return $"Certificate with Subject = {certificate.SubjectName.Name} and Thumbprint = {certificate.Thumbprint} only becomes valid starting from {certificate.NotBefore:F}.";
            }

            var chainErrors = new List<string>();
            try
            {
                var x509ChainPolicy = new X509ChainPolicy
                {
                    RevocationMode = X509RevocationMode.NoCheck,
                };
                using (var chain = new X509Chain { ChainPolicy = x509ChainPolicy})
                {
                    var isValidChain = chain.Build(certificate);

                    if (!isValidChain)
                    {
                        for (var index = 0; index < chain.ChainStatus.Length; index++)
                        {
                            var status = chain.ChainStatus[index];

                            var chainError = $"Certificate with Subject = {certificate.SubjectName.Name} and Thumbprint = {certificate.Thumbprint} chain validation failed: " +
                                $"Chain status [{index}] : {status.Status} {status.StatusInformation}";

                            chainErrors.Add(chainError);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                chainErrors.Add(
                    $"Certificate with Subject = {certificate.SubjectName.Name} and Thumbprint = {certificate.Thumbprint} chain validation failed with an error {e.Message} {e.GetType()} {e.StackTrace}."
                );
            }

            foreach (var error in chainErrors)
                yield return error;
        } 
    }
}