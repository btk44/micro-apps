using System.ComponentModel.DataAnnotations.Schema;

[Table("SomeTable")]
public class SomeEntity{
    public int Id { get; set; }
    public string Description { get; set; }
}