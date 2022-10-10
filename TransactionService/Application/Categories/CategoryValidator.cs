namespace TransactionService.Application.Categories;

public class CategoryValidator {
    public bool IsNameValid(string name){
        return !string.IsNullOrEmpty(name) && name.Length < 200; // to do : consider putting length in config
    }
}