using System;
using System.Collections.Generic;

namespace RequestDropper.Models
{
    /// <summary>
    /// The <see cref="DropperSettings"/> used to check requests against.
    /// </summary>
    public class DropperSettings
    {
        /// <summary>
        /// The message that will be shown when the limited is exceeded and <see cref="Redirect"/> is unset.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The redirect that will be returned when the limited is exceeded.
        /// </summary>
        public string? Redirect { get; set; }

        /// <summary>
        /// The limit that will cause the quotaexceeded message to be returned.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// The period that count will be remembered for.
        /// </summary>
        public TimeSpan Period { get; set; }

        /// <summary>
        /// Specific rules that depending on the statusCode.
        /// </summary>
        public Dictionary<string, DropperRule> Rules { get; set; } = new Dictionary<string, DropperRule>();

        /// <summary>
        /// Paths to ignore.
        /// </summary>
        public List<string> ExcludedPaths { get; set; } = new List<string>();
    }

    /// <summary>
    /// The <see cref="DropperRule"/> speficic rules for a statusCode.
    /// </summary>
    public class DropperRule
    {
        /// <summary>
        /// How much the count should be increased.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Paths to ingore for this statusCode.
        /// </summary>
        public List<string> ExcludedPaths { get; set; } = new List<string>();
    }
}
