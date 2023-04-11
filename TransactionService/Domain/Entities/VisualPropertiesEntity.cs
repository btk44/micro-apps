using TransactionService.Domain.Common.Entities;

public class VisualPropertiesEntity: BaseEntity {
    public string ParentObjectName { get; set; }
    public int ParentObjectId { get; set; }
    public string Color { get; set; } = "#00000";
    public string Icon { get; set; } = string.Empty;
} 