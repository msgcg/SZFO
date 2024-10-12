using Newtonsoft.Json;

namespace ХакатонСЗФО.Controllers
{
    public class ApiResponse
    {
        [JsonProperty("suggestions")]
        public List<Suggestion> Suggestions { get; set; }
    }

    public class Suggestion
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("unrestricted_value")]
        public string UnrestrictedValue { get; set; }

        [JsonProperty("data")]
        public SuggestionData Data { get; set; }
    }

    public class SuggestionData
    {
        [JsonProperty("idx")]
        public string Idx { get; set; }

        [JsonProperty("razdel")]
        public string Razdel { get; set; }

        [JsonProperty("kod")]
        public string Kod { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }


}
