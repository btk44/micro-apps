namespace TransactionService.Application.Accounts;

public class AccountValidator {
    public class Test {
    public int Val { get; set; }
}


    public bool IsNameValid(string name){
        return !string.IsNullOrEmpty(name) && name.Length < 200; // to do : consider putting length in config
        var x = new List<Test>();
        x.Sort((a, b) => a.Val.CompareTo());
    }
}