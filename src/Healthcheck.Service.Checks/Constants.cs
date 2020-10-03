namespace Healthcheck.Service.Core
{
    /// <summary>
    /// Constants class
    /// </summary>
    public struct Constants
    {
        /// <summary>
        /// The healthcheck modules folder identifier
        /// </summary>
        public const string HealthcheckModulesFolderId = "";

        /// <summary>
        /// The components root folder identifier
        /// </summary>
        public const string ComponentsRootFolderId = "{AA16EF54-4954-4B4B-9056-537F97E9EE0F}";

        /// <summary>
        /// The settings item identifier
        /// </summary>
        public const string SettingsItemId = "{857AAA78-61C8-42F7-9CC5-8D5C68EF9FD8}";

        /// <summary>
        /// The master database name
        /// </summary>
        public const string MasterDatabaseName = "master";

        /// <summary>
        /// Template name constants
        /// </summary>
        public struct TemplateNames
        {
            /// <summary>
            /// The item healthcheck template name
            /// </summary>
            public const string ItemHealthcheckTemplateName = "Item Check";

            /// <summary>
            /// The database healthcheck template name
            /// </summary>
            public const string DatabaseHealthcheckTemplateName = "Database Healthcheck";

            /// <summary>
            /// The certificate check template name
            /// </summary>
            public const string CertificateCheckTemplateName = "Certificate Check";

            /// <summary>
            /// The log file check template name
            /// </summary>
            public const string LogFileCheckTemplateName = "Log File Check";

            /// <summary>
            /// The application insight template name
            /// </summary>
            public const string ApplicationInsightTemplateName = "Application Insights Check";

            /// <summary>
            /// The custom healthcheck template name
            /// </summary>
            public const string CustomHealthcheckTemplateName = "Custom Healthcheck";

            /// <summary>
            /// The API health check template name
            /// </summary>
            public const string ApiHealthCheckTemplateName = "API Healthcheck";

            /// <summary>
            /// The basic authentication template name
            /// </summary>
            public const string BasicAuthenticationTemplateName = "Basic Authentication";

            /// <summary>
            /// The json web token authentication template name
            /// </summary>
            public const string JsonWebTokenAuthentication = "JSON Web Token Authentication";

            /// <summary>
            /// The certificate authentication template name
            /// </summary>
            public const string CertificateAuthentication = "Certificate Authentication";

            /// <summary>
            /// The search index health check template name
            /// </summary>
            public const string SearchIndexHealthCheckTemplateName = "Search Healthcheck";

            /// <summary>
            /// The license check template name
            /// </summary>
            public const string LicenseCheckTemplateName = "License Check";

            /// <summary>
            /// The x connect API check template name
            /// </summary>
            public const string XConnectApiCheckTemplateName = "XConnectAPI Check";

            /// <summary>
            /// The windows service check template name
            /// </summary>
            public const string WindowsServiceCheckTemplateName = "Windows Service Check";

            /// <summary>
            /// The web job check template name
            /// </summary>
            public const string WebJobCheckTemplateName = "Web Job Check";

            /// <summary>
            /// The keep alive template name
            /// </summary>
            public const string KeepAliveTemplateName = "KeepAlive Check";

            /// <summary>
            /// The queue check template name
            /// </summary>
            public const string QueueCheckTemplateName = "Queue Check";

            /// <summary>
            /// The local disk space check template name
            /// </summary>
            public const string LocalDiskSpaceCheckTemplateName = "LocalDisk Space Check";

            /// <summary>
            /// The spe check template name
            /// </summary>
            public const string SPECheckTemplateName = "SPE Check";

            /// <summary>
            /// The remote log file check template name
            /// </summary>
            public const string RemoteLogFileCheckTemplateName = "Remote Log File Check";

            /// <summary>
            /// The remote certificate check template name
            /// </summary>
            public const string RemoteCertificateCheckTemplateName = "Remote Certificate Check";

            public const string RemoteXConnectApiCheckTemplateName = "Remote XConnectAPI Check";

            public const string RemoteApiHealthcheckTemplateName= "Remote API Healthcheck";

            public const string RemoteDatabaseHealtcheckTemplateName = "Remote Database Healthcheck";

            public const string RemoteCustomHealthcheckTemplateName = "Remote Custom Healthcheck";

            public const string RemoteLicenseHealthcheckTemplateName = "Remote License Check";

            public const string RemoteDiskSpaceCheckTemplateName = "Remote Disk Space Check";

            public const string RemoteWindowsServiceCheckTemplateName = "Remote Windows Service Check";
        }
    }
}