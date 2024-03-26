namespace Burg.FrontEnd.AST;

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

public enum StatementType
{
    Chunk,

    ValDeclaration,
    FnDeclaration,

    ReturnStmt,
    IfStmt,

    Identifier,
    MemberExpr,
    CallExpr,
    BinaryExpr,

    IntegerLit,
    FloatLit,
    StringLit,
    DictionaryLit,
    DictionaryProperty,
    ArrayLit,
    LambdaLit
}

public interface IStatement
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    StatementType Kind { get; }

    public string? ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
}

public struct Chunk : IStatement
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.Chunk;
    public List<IStatement> body;

    public Chunk(List<IStatement> body)
    {
        this.body = body;
    }
}

public struct ValDeclaration : IStatement
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.ValDeclaration;
    public Identifier identifier;
    public IExpression value;

    public ValDeclaration(Identifier identifier, IExpression value)
    {
        this.identifier = identifier;
        this.value = value;
    }
}

public struct FnDeclaration : IStatement
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.FnDeclaration;
    public Identifier identifier;
    public Chunk body;
    public List<Identifier>? parameters;

    public FnDeclaration(Identifier identifier, Chunk body, List<Identifier>? parameters = null)
    {
        this.identifier = identifier;
        this.body = body;
        this.parameters = parameters;
    }
}

public struct ReturnStmt : IStatement
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.ReturnStmt;
    public IExpression value;

    public ReturnStmt(IExpression value)
    {
        this.value = value;
    }
}

public struct IfStmt : IStatement
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.IfStmt;
    public IExpression test;
    public Chunk consequent;
    public IStatement? alternate;

    public IfStmt(IExpression test, Chunk consequent, IStatement? alternate = null)
    {
        this.test = test;
        this.consequent = consequent;
        this.alternate = alternate;
    }
}

public interface IExpression : IStatement { }

public struct Identifier : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.Identifier;
    public string symbol;

    public Identifier(string symbol)
    {
        this.symbol = symbol;
    }
}

public struct MemberExpr : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.MemberExpr;
    public IExpression obj;
    public IExpression property;
    public bool isComputed;

    public MemberExpr(IExpression obj, IExpression property, bool isComputed = false)
    {
        this.obj = obj;
        this.property = property;
        this.isComputed = isComputed;
    }
}

public struct CallExpr : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.CallExpr;
    public IExpression caller;
    public List<IExpression>? args;

    public CallExpr(IExpression caller, List<IExpression>? args = null)
    {
        this.caller = caller;
        this.args = args;
    }
}

public struct BinaryExpr : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.BinaryExpr;
    public IExpression left;
    public string opr;
    public IExpression right;

    public BinaryExpr(IExpression left, string opr, IExpression right)
    {
        this.left = left;
        this.opr = opr;
        this.right = right;
    }
}

public struct IntegerLit : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.IntegerLit;
    public int value;

    public IntegerLit(int value)
    {
        this.value = value;
    }
}

public struct FloatLit : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.FloatLit;
    public double value;

    public FloatLit(double value)
    {
        this.value = value;
    }
}

public struct StringLit : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.StringLit;
    public string value;

    public StringLit(string value)
    {
        this.value = value;
    }
}

public struct DictionaryLit : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.DictionaryLit;
    
    [JsonProperty("props")]
    public List<DictionaryProperty> props;
}

public struct DictionaryProperty : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.DictionaryProperty;
    public IExpression key;
    public IExpression value;
}

public struct ArrayLit : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.ArrayLit;
    public List<IExpression> list;
}

public struct LambdaLit : IExpression
{
    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    readonly StatementType IStatement.Kind => StatementType.LambdaLit;
    public Chunk body;
    public List<Identifier>? parameters;

    public LambdaLit(Chunk body, List<Identifier>? parameters = null)
    {
        this.body = body;
        this.parameters = parameters;
    }
}

