using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
namespace AlertsApi.Models;

public class AlertModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [Required(AllowEmptyStrings = false)]
    [BsonElement("alert_id")]
    [JsonPropertyName("alert_id")]
    public string AlertId { get; set; } = string.Empty;
    [JsonPropertyName("source")]
    [BsonElement("aource")]
    [RegularExpression("^(aman|mossad|pikud-haoref|shabak)$")]
    [Required]
    public string Source { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("title")]
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("content")]
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;
    [JsonPropertyName("priority")]
    [BsonElement("priority")]
    [Required(AllowEmptyStrings = false)]
    [RegularExpression("^(CRITICAL|HIGH|MEDIUM|LOW)^")]
    public string Priority { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    [JsonPropertyName("classification")]
    [BsonElement("classification")]
    [RegularExpression("^(UNCLASSIFIED|RESTRICTED|SECRET|TOP_SECRET)^")]
    public string Classification { get; set; } = string.Empty;
    [Required]
    [Range(-90, 90)]
    [JsonPropertyName("lat")]
    [BsonElement("lat")]
    public double Lat { get; set; }
    [Required]
    [JsonPropertyName("lon")]
    [BsonElement("lon")]
    [Range(-180, 180)]
    public double Lon { get; set; }
    [Required]
    [JsonPropertyName("timestamp")]
    [BsonElement("timestamp")]
    public DateTime TimeStamp { get; set; }
    [JsonPropertyName("status")]
    [BsonElement("status")]
    [Required(AllowEmptyStrings = false)]
    public string Status { get; set; } = "WAITING";
}
