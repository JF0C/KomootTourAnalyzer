using System;
using System.Text.Json.Serialization;

namespace KomootTourAnalyzer.DTOs
{
	public class TourDto
	{
		[JsonPropertyName("status")]
		public string Status { get; set; } = string.Empty;

		[JsonPropertyName("date")]
		public DateTime Date { get; set; }

		[JsonPropertyName("distance")]
		public double DistanceInMeters { get; set; }

		[JsonPropertyName("time_in_motion")]
		public int SecondsInMotion { get; set; }

		[JsonPropertyName("duration")]
		public int SecondsTotal { get; set; }

		[JsonPropertyName("elevation_up")]
		public double ElevationUpInMeters { get; set; }

		[JsonPropertyName("elevation_down")]
		public double ElevationDownInMeters { get; set; }
	}
}

