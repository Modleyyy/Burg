namespace Burg.FrontEnd.Tokenizing;

public record Token
{
    public readonly TokenType type;
    public readonly string raw;

    public Token(TokenType type, string raw) {
        this.type = type;
        this.raw = raw;
    }

    public Token(TokenType type, char raw)
    {
        this.type = type;
        this.raw = raw.ToString();
    }

    public override string ToString()
    {
        return "Token: { Type: " + type + ", Raw text: \"" + raw + "\" }";
    }
}
