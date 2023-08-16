using System;
using System.Text.Json.Serialization;

namespace KomootTourAnalyzer.DTOs
{
	public class TourContainerDto
	{
		[JsonPropertyName("tours")]
		public List<TourDto> Tours { get; set; } = new();
	}
}

