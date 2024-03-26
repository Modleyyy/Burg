namespace Burg.FrontEnd.Tokenizing;

public static class Tokenizer
{
    public static List<Token> Tokenize(string sourceCode) {
        List<char> src = sourceCode.ToCharArray().ToList();
        List<Token> tokens = new();

        while (src.Count > 0)
        {
            switch (src[0])
            {
                case ' ': case '\t': case '\n': case '\r':
                    src.RemoveAt(0);
                    break;
                case '(':
                    tokens.Add(new(TokenType.OpenParen, src[0]));
                    src.RemoveAt(0);
                    break;
                case ')':
                    tokens.Add(new(TokenType.CloseParen, src[0]));
                    src.RemoveAt(0);
                    break;
                case '{':
                    tokens.Add(new(TokenType.OpenBrace, src[0]));
                    src.RemoveAt(0);
                    break;
                case '}':
                    tokens.Add(new(TokenType.CloseBrace, src[0]));
                    src.RemoveAt(0);
                    break;
                case '[':
                    tokens.Add(new(TokenType.OpenBracket, src[0]));
                    src.RemoveAt(0);
                    break;
                case ']':
                    tokens.Add(new(TokenType.CloseBracket, src[0]));
                    src.RemoveAt(0);
                    break;
                case '.':
                    if (IsDigit(src[1]))
                    {
                        src.RemoveAt(0); // remove the . character
                        string number = "0.";
                        while (IsDigit(src[0]))
                        {
                            number += src[0];
                            src.RemoveAt(0);
                        }
                        tokens.Add(new(TokenType.FloatLit, number));
                    }
                    else
                    {
                        tokens.Add(new(TokenType.Accessor, src[0]));
                        src.RemoveAt(0);
                    }
                    break;
                case ',':
                    tokens.Add(new(TokenType.Seperator, src[0]));
                    src.RemoveAt(0);
                    break;
                case ';':
                    tokens.Add(new(TokenType.EndLine, src[0]));
                    src.RemoveAt(0);
                    break;
                case '+': case '-': case '*': case '/': case '%': case '^':
                    tokens.Add(new(TokenType.BinaryOperator, src[0]));
                    src.RemoveAt(0);
                    break;
                case '#':
                    while (src.Count > 0 && src[0] != '\n')
                        src.RemoveAt(0);
                    break;
                case '$':
                    src.RemoveAt(0); // remove opening $
                    while (src.Count > 0 && src[0] != '$')
                        src.RemoveAt(0); // remove everything inbetween the $s
                    src.RemoveAt(0); // remove closing $
                    break;

                default:
                    if (IsDigit(src[0]))
                    {
                        string num = "";
                        bool hasDecimal = false;

                        while (src.Count > 0 && (IsDigit(src[0]) || src[0] == '.'))
                        {
                            if (src[0] == '.')
                            {
                                if (hasDecimal)
                                    throw new("Invalid number format");
                                hasDecimal = true;
                            }
                            num += src[0];
                            src.RemoveAt(0);
                        }

                        if (hasDecimal || num.Contains('.'))
                            tokens.Add(new(TokenType.FloatLit, num));
                        else
                            tokens.Add(new(TokenType.IntegerLit, num));
                    }
                    else if (IsAlphabetic(src[0]) || src[0] == '_')
                    {
                        string word = "";
                        while (src.Count > 0 && (IsAlphabetic(src[0]) || src[0] == '_'))
                        {
                            word += src[0];
                            src.RemoveAt(0);
                        }

                        switch (word)
                        {
                            case "val":
                                tokens.Add(new(TokenType.Val, word));
                                break;
                            case "fn":
                                tokens.Add(new(TokenType.Function, word));
                                break;
                            case "lm":
                                tokens.Add(new(TokenType.Lambda, word));
                                break;
                            case "return":
                                tokens.Add(new(TokenType.Return, word));
                                break;
                            case "if":
                                tokens.Add(new(TokenType.If, word));
                                break;
                            case "then":
                                tokens.Add(new(TokenType.Then, word));
                                break;
                            case "else":
                                tokens.Add(new(TokenType.Else, word));
                                break;
                            case "end":
                                tokens.Add(new(TokenType.End, word));
                                break;
                            default:
                                tokens.Add(new(TokenType.Identifier, word));
                                break;
                        }
                    }
                    else if (src[0] == '"')
                    {
                        src.RemoveAt(0); // pass the opening " character
                        string strcontent = "";
                        
                        while (src[0] != '"' && src.Count > 0)
                        {
                            strcontent += src[0];
                            src.RemoveAt(0);
                        }

                        if (src[0] != '"')
                            throw new("Tokenizer Error:\n String not closed. Expected: \" Got: EOF");

                        src.RemoveAt(0);

                        tokens.Add(new(TokenType.StringLit, strcontent));
                    }
                    else if (src[0] == '=' || src[0] == '<' || src[0] == '>' ||
                             src[0] == '|' || src[0] == '&' || src[0] == '!') 
                    {
                        string op = "";
                        while (src.Count > 0 && (src[0] == '=' || src[0] == '<' ||
                                                 src[0] == '>' || src[0] == '|' ||
                                                 src[0] == '&' || src[0] == '!'))
                        {
                            op += src[0];
                            src.RemoveAt(0);
                        }

                        if (op == "==" || op == "!=" || op == "<"  ||
                            op == ">"  || op == "<=" || op == ">=" ||
                            op == "||" || op == "&&")
                        {
                            tokens.Add(new(TokenType.BinaryOperator, op));
                        }
                        else if (op == "=")
                        {
                            tokens.Add(new(TokenType.SetValue, op));
                        }
                        else
                        {
                            throw new("Tokenizer Error:\n Unknown operator type: " + op);
                        }
                    } else {
                        throw new("Tokenizer Error:\n Unknown token: " + src[0]);
                    }
                    break;
            }
        }

        tokens.Add(new(TokenType.EOF, "EOF"));
        return tokens;
    }
    private static bool IsAlphabetic(char c) {
        return char.ToUpper(c) != char.ToLower(c);
    }

    private static bool IsDigit(char c) {
        return char.IsDigit(c);
    }
}
