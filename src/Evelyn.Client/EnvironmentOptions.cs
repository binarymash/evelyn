namespace Evelyn.Client
{
    using System;

    public class EnvironmentOptions
    {
        /// <summary>Configures project that we are retrieving toggles for</summary>
        /// <value>The project that we are retrieving toggles for</value>
        public Guid ProjectId { get; set; }

        /// <summary>Configures the environment that we are retrieving toggles for</summary>
        /// <value>The environment that we are retrieving toggles for</value>
        public string Environment { get; set; }
    }
}
