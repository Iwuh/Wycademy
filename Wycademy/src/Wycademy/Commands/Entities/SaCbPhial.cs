using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wycademy.Commands.Entities
{
    public class SaCbPhial
    {
        /// <summary>
        /// The phial type.
        /// </summary>
        [JsonProperty]
        public string Type { get; set; }

        /// <summary>
        /// The phial's value, if applicable.
        /// </summary>
        [JsonProperty]
        public string Value { get; set; }
    }
}
