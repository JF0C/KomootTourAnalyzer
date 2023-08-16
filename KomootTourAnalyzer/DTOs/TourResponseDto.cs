using System;
using System.Text.Json.Serialization;

namespace KomootTourAnalyzer.DTOs
{
	public class TourResponseDto
	{
		[JsonPropertyName("_embedded")]
		public TourContainerDto Embedded { get; set; } = new();

		[JsonPropertyName("page")]
		public PageData Page { get; set; } = new();
	}
}

