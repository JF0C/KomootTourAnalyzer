using System;
using System.Text.Json.Serialization;

namespace KomootTourAnalyzer.DTOs
{
	public class PageData
	{
		[JsonPropertyName("size")]
		public int Size { get; set; }

		[JsonPropertyName("totalElements")]
		public int TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("number")]
        public int PageNumber { get; set; }
    }
}

