namespace Burg.Runtime;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public enum ValueTypes
{
    Null,
    Integer,
    Float,
    String,
    Bool,
    Dictionary,
    Array,
    Function,
    Return
}

public interface IRuntimeValue
{
    [JsonProperty(nameof(type))]
    [JsonConverter(typeof(StringEnumConverter))]
    ValueTypes type { get; }
    [JsonProperty(nameof(value))]
    object value { get; }
}

public struct NullValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.Null;
    readonly object IRuntimeValue.value => "null";
}

public  struct IntegerValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.Integer;
    readonly object IRuntimeValue.value => value;
    public int value;

    public IntegerValue(int value)
    {
        this.value = value;
    }
}

public struct FloatValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.Float;
    readonly object IRuntimeValue.value => value;
    public double value;

    public FloatValue(double value)
    {
        this.value = value;
    }
}

public struct StringValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.String;
    readonly object IRuntimeValue.value => value;
    public string value;

    public StringValue(string value)
    {
        this.value = value;
    }
}

public struct BoolValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.Bool;
    readonly object IRuntimeValue.value => value;
    public bool value;

    public BoolValue(bool value)
    {
        this.value = value;
    }
}

public struct DictionaryValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.Dictionary;
    public readonly object value => JsonConvert.SerializeObject(props, Formatting.Indented);

    public Dictionary<IRuntimeValue, IRuntimeValue> props;
}

public struct ArrayValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.Array;
    public readonly object value => JsonConvert.SerializeObject(list, Formatting.Indented);
    public List<IRuntimeValue> list;

    public ArrayValue(List<IRuntimeValue> list)
    {
        this.list = list;
    }
}

public struct FunctionValue : IRuntimeValue
{
    public delegate IRuntimeValue Function(List<IRuntimeValue> args);
    public readonly ValueTypes type => ValueTypes.Function;
    public readonly object value => call;

    public Function call;

    public FunctionValue(Function call)
    {
        this.call = call;
    }
}

public struct ReturnValue : IRuntimeValue
{
    public readonly ValueTypes type => ValueTypes.Return;

    readonly object IRuntimeValue.value => value;

    public IRuntimeValue value;
}
