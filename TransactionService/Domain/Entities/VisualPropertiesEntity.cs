using TransactionService.Domain.Common.Entities;

public class VisualPropertiesEntity: BaseEntity {
    public string Color { get; set; } = "#00000";
    public string Icon { get; set; } = string.Empty;
} 