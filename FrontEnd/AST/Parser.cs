namespace Burg.FrontEnd.AST;

using Burg.FrontEnd.Tokenizing;
using System.Globalization;

public static class Parser
{
    public static Chunk ParseAST(List<Token> tokens)
    {
        List<IStatement> body = new();

        while (!ReachedEOF(tokens))
        {
            body.Add(ParseStmt(tokens));
        }

        return new Chunk(body);
    }

    private static IStatement ParseStmt(List<Token> tokens)
    {
        Token tk = tokens[0];
        var checksemcol = () => {
            Expect(TokenType.EndLine, tokens, "Must end statement by semicolon. Expected: ; Got: " + tokens[0].raw);
        };

        if (tk.type == TokenType.Val)
        {
            tokens.RemoveAt(0); // remove the val keyword
            Token identifier = Expect(TokenType.Identifier, tokens,
                "Expected value name after val keyword. Expected: Identifier Got: " + tokens[0].raw);

            Expect(TokenType.SetValue, tokens,
                "Expected = after val definition. Expected: = Got: " + tokens[0].raw);

            IExpression value = ParseExpr(tokens);

            checksemcol();

            return new ValDeclaration(new(identifier.raw), value);
        }
        else if (tk.type == TokenType.Function)
        {
            tokens.RemoveAt(0); // remove the fn keyword

            Token identifier = Expect(TokenType.Identifier, tokens,
                "Expected function name after fn keyword. Expected: Identifier Got: " + tokens[0].raw);

            Expect(TokenType.OpenParen, tokens,
                "Expected opening parenthesis after function name. Expected: ( Got: " + tokens[0].raw);

            List<Identifier> parms = new();

            bool firstIter = true;

            while (tokens[0].type != TokenType.CloseParen)
            {
                if (!firstIter)
                    Expect(TokenType.Seperator, tokens,
                        "Expected seperator , between parameter definitions. Expected: , Got: " + tokens[0].raw);

                firstIter = false;

                parms.Add(new(Expect(TokenType.Identifier, tokens,
                            "Expected parameter identifier. Expected: Identifier Got: " + tokens[0].raw).raw));
            }

            Expect(TokenType.CloseParen, tokens,
                "Expected closing parenthesis after parameters. Expected: ) Got: " + tokens[0].raw);

            List<IStatement> body = new();

            while (tokens[0].type != TokenType.End)
                body.Add(ParseStmt(tokens));

            Expect(TokenType.End, tokens, "wait how the fuck did you get here");
            Expect(TokenType.Function, tokens,
                "Must specify what to end (fn). Expected: fn Got: " + tokens[0].raw);

            return new FnDeclaration(new(identifier.raw), new(body), parms);
        }
        else if (tk.type == TokenType.Return)
        {
            tokens.RemoveAt(0); // remove the return keyword
            IExpression value = ParseExpr(tokens);
            checksemcol();
            return new ReturnStmt(value);
        }
        else if (tk.type == TokenType.If)
        {
            return ParseIfStmt(tokens, false);
        }
        IExpression expr = ParseExpr(tokens);
        checksemcol();

        return expr;
    }
    private static IfStmt ParseIfStmt(List<Token> tokens, bool isAlternate) {
        tokens.RemoveAt(0); // remove the if keyword

        Expect(TokenType.OpenParen, tokens,
            "Expected opening parenthesis after if keyword. Expected: ( Got: " + tokens[0].raw);

        IExpression test = ParseExpr(tokens);

        Expect(TokenType.CloseParen, tokens,
            "Expected closing parenthesis after if check. Expected: ) Got: " + tokens[0].raw);

        Expect(TokenType.Then, tokens,
            "Expected then keyword after if statement. Expected: then Got: " + tokens[0].raw);

        List<IStatement> body = new();

        while (tokens[0].type != TokenType.End && tokens[0].type != TokenType.Else)
            body.Add(ParseStmt(tokens));
        
        IStatement? alternate = null;
        
        if (tokens[0].type == TokenType.Else)
        {
            tokens.RemoveAt(0); // remove else keyword

            if (tokens[0].type == TokenType.If) // else if
            {
                alternate = ParseIfStmt(tokens, true);
            }
            else // normal else
            {
                List<IStatement> elsebody = new();

                while (tokens[0].type != TokenType.End)
                    elsebody.Add(ParseStmt(tokens));
                alternate = new Chunk(elsebody);
            }
        }
        if (!isAlternate) {
            if (tokens[0].type == TokenType.End) {
                tokens.RemoveAt(0); // remove end keyword
                Expect(TokenType.If, tokens,
                    "Must specify what to end (if). Expected: if Got: " + tokens[0].raw);
            } else {
                throw new("Parser Error:\n Expected end keyword after if statement. Expected: end Got:" + tokens[0].raw);
            }
        }
        
        return new IfStmt(test, new(body), alternate);
    }

    private static IExpression ParseExpr(List<Token> tokens)
    {
        return ParseBooleanExpr(tokens);
    }

    private static IExpression ParseBooleanExpr(List<Token> tokens)
    {
        IExpression left = ParseLambdaExpr(tokens);

        while (tokens[0].type == TokenType.BinaryOperator &&
              (tokens[0].raw == "==" || tokens[0].raw == "!=" || tokens[0].raw == "<" ||
               tokens[0].raw == ">"  || tokens[0].raw == "<=" || tokens[0].raw == ">=" ||
               tokens[0].raw == "&&" || tokens[0].raw == "||"))
        {
            string oper = tokens[0].raw;
            tokens.RemoveAt(0);

            IExpression right = ParseLambdaExpr(tokens);
            left = new BinaryExpr(left, oper, right);
        }

        return left;
    }

    private static IExpression ParseLambdaExpr(List<Token> tokens)
    {
        if (tokens[0].type != TokenType.Lambda)
            return ParseDictionaryExpr(tokens);

        tokens.RemoveAt(0); // remove the lm keyword

        Expect(TokenType.OpenParen, tokens,
            "Expected opening parenthesis after lm keyword. Expected: ( Got: " + tokens[0].raw);

        List<Identifier> parms = new();

        bool firstIter = true;

        //@ts-expect-errors
        while (tokens[0].type != TokenType.CloseParen)
        {
            if (!firstIter)
                Expect(TokenType.Seperator, tokens,
                    "Expected seperator , between parameter definitions. Expected: , Got: " + tokens[0].raw);

            firstIter = false;

            parms.Add(new(Expect(TokenType.Identifier, tokens,
                        "Expected parameter identifier. Expected: Identifier Got: " + tokens[0].raw).raw));
        }

        Expect(TokenType.CloseParen, tokens,
            "Expected closing parenthesis after parameters. Expected: ) Got: " + tokens[0].raw);

        List<IStatement> body = new();

        while (tokens[0].type != TokenType.End)
            body.Add(ParseStmt(tokens));

        Expect(TokenType.End, tokens, "wait how the fuck did you get here");
        Expect(TokenType.Lambda, tokens,
            "Must specify what to end (lm). Expected: lm Got: " + tokens[0].raw);

        return new LambdaLit(new(body), parms);
    }

    private static IExpression ParseDictionaryExpr(List<Token> tokens)
    {
        if (tokens[0].type != TokenType.OpenBrace)
            return ParseArrayExpr(tokens);

        tokens.RemoveAt(0);

        List<DictionaryProperty> props = new();

        while (!ReachedEOF(tokens))
        {
            Token tk = tokens[0];
            tokens.RemoveAt(0);
            if (tk.type == TokenType.CloseBrace)
                break;
            if (tk.type == TokenType.Identifier)
            {
                Expect(TokenType.SetValue, tokens,
                    "Expected = after property key definition. Expected: = Got: " + tokens[0].raw);
                IExpression value = ParseExpr(tokens);
                props.Add(new() { key = new Identifier(tk.raw), value = value });

                Token s = tokens[0];
                tokens.RemoveAt(0);
                if (s.type == TokenType.Seperator)
                    continue;
                else if (s.type == TokenType.CloseBrace)
                    break;
                else
                    throw new(
                        "Parser Error:\n Expected seperator , or closing brace } after property definition. Expected: , or } Got: "
                        + s.raw);
            }
            else if (tk.type == TokenType.OpenBrace)
            {
                IExpression key = ParseExpr(tokens);
                Expect(TokenType.CloseBracket, tokens,
                    "Expected ] after computed dictionary key definition. Expected: } Got: " + tokens[0].raw);
                Expect(TokenType.SetValue, tokens,
                    "Expected = after property key definition. Expected: = Got: " + tokens[0].raw);
                IExpression value = ParseExpr(tokens);

                props.Add(new() { key = key, value = value} );

                Token s = tokens[0];
                tokens.RemoveAt(0);
                if (s.type == TokenType.Seperator) {
                    continue;
                } else if (s.type == TokenType.CloseBrace) {
                    break;
                } else {
                    throw new(
                        "Parser Error:\n Expected seperator , or closing brace } after property definition. Expected: , or } Got: "
                        + s.raw);
                }
            }
        }

        return new DictionaryLit() { props = props };
    }

    private static IExpression ParseArrayExpr(List<Token> tokens)
    {
        if (tokens[0].type != TokenType.OpenBracket)
            return ParseMultiplicativeExpr(tokens);

        tokens.RemoveAt(0);

        List<IExpression> list = new();

        while (!ReachedEOF(tokens)) {
            if (tokens[0].type == TokenType.CloseBracket)
                break;

            list.Add(ParseExpr(tokens));

            Token s = tokens[0];
            tokens.RemoveAt(0);
            if (s.type == TokenType.Seperator) {
                continue;
            } else if (s.type == TokenType.CloseBracket) {
                break;
            } else {
                throw new(
                    "Parser Error:\n Expected seperator , or closing bracket ] after property definition. Expected: , or ] Got: "
                    + s.raw);
            }
        }

        return new ArrayLit() { list = list };
    }

    private static IExpression ParseMultiplicativeExpr(List<Token> tokens)
    {
        IExpression left = ParseAdditiveExpr(tokens);

        while (tokens[0].type == TokenType.BinaryOperator &&
            (tokens[0].raw == "*" || tokens[0].raw == "/" || tokens[0].raw == "%" || tokens[0].raw == "^"))
        {
            string oper = tokens[0].raw;
            tokens.RemoveAt(0);

            IExpression right = ParseAdditiveExpr(tokens);
            left = new BinaryExpr(left, oper, right);
        }

        return left;
    }

    private static IExpression ParseAdditiveExpr(List<Token> tokens)
    {
        IExpression left = ParseCallMemberExpr(tokens);

        while (tokens[0].type == TokenType.BinaryOperator && (tokens[0].raw == "+" || tokens[0].raw == "-"))
        {
            string oper = tokens[0].raw;
            tokens.RemoveAt(0);

            IExpression right = ParseCallMemberExpr(tokens);
            left = new BinaryExpr(left, oper, right);
        }

        return left;
    }

    private static IExpression ParseCallMemberExpr(List<Token> tokens)
    {
        IExpression member = ParseMemberExpr(tokens);

        while (tokens[0].type == TokenType.OpenParen)
            member = ParseCallExpr(tokens, member);

        return member;
    }

    private static IExpression ParseMemberExpr(List<Token> tokens)
    {
        IExpression member = ParsePrimaryExpr(tokens);

        bool isComputed;
        IExpression property;

        while (tokens[0].type == TokenType.Accessor || tokens[0].type == TokenType.OpenBracket) {
            if (tokens[0].type == TokenType.Accessor) { // Normal
                tokens.RemoveAt(0); // remove the .
                isComputed = false;
                property = ParsePrimaryExpr(tokens);

                if (property.Kind != StatementType.Identifier)
                {
                    throw new(
                        "Parser Error:\n Cannot use the . member accessor without an identifier after it. Expected: Identifier Got: " +
                        property.Kind);
                }
            } else { // Computed
                tokens.RemoveAt(0); // remove the [
                isComputed = true;
                property = ParseExpr(tokens);

                Expect(TokenType.CloseBracket, tokens,
                    "Missing closing bracket after computed member accessing. Expected: ] Got: " + tokens[0].raw);
            }

            member = new MemberExpr() { isComputed = isComputed, obj = member, property = property };
        }

        return member;
    }

    private static CallExpr ParseCallExpr(List<Token> tokens, IExpression caller)
    {
        CallExpr callexpr = new() {
            caller = caller,
            args = ParseCallArgsExpr(tokens)
        };

        while (tokens[0].type == TokenType.OpenParen) 
            callexpr = ParseCallExpr(tokens, caller);

        return callexpr;
    }

    private static List<IExpression> ParseCallArgsExpr(List<Token> tokens)
    {
        Expect(TokenType.OpenParen, tokens, "wait how did you even get here");
        List<IExpression> args = new();
        bool firstIter = true;

        while (tokens[0].type != TokenType.CloseParen)
        {
            if (!firstIter)
                Expect(TokenType.Seperator, tokens,
                    "Expected seperator , between argument expressions. Expected: , Got: " + tokens[0].raw);

            firstIter = false;

            args.Add(ParseExpr(tokens));
        }

        Token s = tokens[0];
        tokens.RemoveAt(0);
        if (s.type == TokenType.Seperator)
            tokens.RemoveAt(0);
        else if (s.type == TokenType.CloseParen) {} // just do nothing lol
        else
            throw new(
                "Parser Error:\n Expected seperator , or closing parenthesis after call expression arguments. Expected: , or ) Got: "
                + s.raw);
        return args;
    }

    private static IExpression ParsePrimaryExpr(List<Token> tokens)
    {
        Token tk = tokens[0];
        switch (tk.type)
        {
            case TokenType.Identifier:
                tokens.RemoveAt(0);
                return new Identifier(tk.raw);
            case TokenType.OpenParen:
                tokens.RemoveAt(0);
                IExpression v = ParseExpr(tokens);
                Expect(TokenType.CloseParen, tokens, "Expected closing parenthesis ). Expected: ) Got: " + tokens[0].raw);
                return v;
            case TokenType.IntegerLit:
                tokens.RemoveAt(0);
                return new IntegerLit(int.Parse(tk.raw, NumberStyles.Integer, CultureInfo.InvariantCulture));
            case TokenType.FloatLit:
                tokens.RemoveAt(0);
                return new FloatLit(double.Parse(tk.raw, NumberStyles.Number, CultureInfo.InvariantCulture));
            case TokenType.StringLit:
                tokens.RemoveAt(0);
                return new StringLit(tk.raw);

            default:
                throw new("Parser Error:\n Unexpected token type: " + tk.type);
        }
    }


    private static bool ReachedEOF(List<Token> tokens)
    {
        return tokens[0].type == TokenType.EOF;
    }

    private static Token Expect(TokenType expectedType, List<Token> tokens, string errorMessage)
    {
        Token tk = tokens[0];
        if (tk.type != expectedType) {
            throw new("Parser Error:\n " + errorMessage);
        }
        tokens.RemoveAt(0);
        return tk;
    }
}
