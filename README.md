# CmdArgs
Full example

public class ConfFullExample
{
    [SwitchArgument('', "SwitchOne", "some description")]
    public bool SwitchOne;
    
    [SwitchArgument('', "SwitchTwo", "some description")]
    public bool SwitchTwo;

    [ValuedArgument('')]
    public int IntField;
    
    [ValuedArgument('', DefaultValue = 5)]
    public int IntDefault;

    [ValuedArgument('')]
    public int IntProp {get;set;}
    
    [ValuedArgument('')]
    public int? IntNullable;
    
    [ValuedArgument('')]
    public int[] IntArray;
    
    [ValuedArgument('')]
    public List<int> IntList;
    
    [ValuedArgument('')]
    public string S;
    
    [ValuedArgument('')]
    public char C;

    [ValuedArgument('')]
    public decimal Dec;
    
    [ValuedArgument('', RegularExpression = @"^[a-d]\w{5,7}$")]
    public string StringRegex;
    
    [ValuedArgument('', RegularExpression = @"^[123]0[89]$")]
    public int IntRegex;
    
    [ValuedArgument('')]
    public int IntPredicated;
    
    public static Predicate<int> IntPredicated_Predicate_closure;    
    public static Predicate<int> IntPredicated_Predicate_one = i => i > 2; // const predicate
    public static Predicate<int[]> IntPredicated_Predicate_ar = i => i.Length > 2; // const predicate
    
    
    [ValuedArgument('', AllowedValues = new object[] {"asd", "rt"}, DefaultValue = "rt")]
    public string StringAllowed;
 
    [ValuedArgument('a', AllowedValues = new object[] {MyEnum.One, MyEnum.Three})]
    public MyEnum MyEnumVal;
 
}

public enum MyEnum : byte
{
    Nol,
    One,
    Two,
    Three
}
