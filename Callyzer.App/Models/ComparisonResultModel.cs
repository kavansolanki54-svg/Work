using System;
using System.Collections.Generic;

namespace Callyzer.App.Models
{
    /// <summary>
    /// Represents the result of comparing multiple contacts side-by-side.
    /// </summary>
    public class ComparisonResultModel
    {
        /// <summary>Start of the comparison period.</summary>
        public DateTime From { get; set; }

        /// <summary>End of the comparison period.</summary>
        public DateTime To { get; set; }

        /// <summary>Analytics data for each compared contact.</summary>
        public List<ContactAnalyticsModel> Contacts { get; set; } = new();
    }
}
