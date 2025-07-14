namespace ProductFunctionsApp.Application.DTOs;

// DTO for queue output after processing
public class ProductProcessedDto
{
    public Guid ProductId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}
