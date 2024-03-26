namespace Burg.Runtime;

using Burg.FrontEnd.AST;

public static class Interpreter
{
    public static IRuntimeValue EvaluateBiExpr(BinaryExpr biexpr, Environment env)
    {
        IRuntimeValue left = EvaluateExpr(biexpr.left, env);
        IRuntimeValue right = EvaluateExpr(biexpr.right, env);

        var typeError = () => {
            Console.WriteLine("Runtime Error:\n Binary Expression type error:");
            Console.WriteLine("\tLeft type: " + left.type.ToString());
            Console.WriteLine("\tRight type: " + right.type.ToString());
            throw new();
        };
        var operatorError = () => {
            Console.WriteLine("Runtime Error:\n Binary Expression operator error:");
            Console.WriteLine("\tOperator: " + biexpr.opr);
            Console.WriteLine("\tLeft type: " + left.type.ToString());
            Console.WriteLine("\tRight type: " + right.type.ToString());
            throw new();
        };

        switch (left.type) {
            case ValueTypes.Integer:
                switch (right.type) {
                    case ValueTypes.Integer:
                        switch (biexpr.opr) {
                            case "+":
                                return new IntegerValue(Convert.ToInt32(left.value) + Convert.ToInt32(right.value));
                            case "-":
                                return new IntegerValue(Convert.ToInt32(left.value) - Convert.ToInt32(right.value));
                            case "*":
                                return new IntegerValue(Convert.ToInt32(left.value) * Convert.ToInt32(right.value));
                            case "/":
                                if (Convert.ToInt32(left.value) == Convert.ToInt32(right.value))
                                    return new IntegerValue(1);
                                else
                                    return new IntegerValue(Convert.ToInt32(left.value) / Convert.ToInt32(right.value));
                            case "%":
                                return new IntegerValue(Convert.ToInt32(left.value) % Convert.ToInt32(right.value));
                            case "^":
                                return new IntegerValue(Convert.ToInt32(Math.Pow(Convert.ToInt32(left.value), Convert.ToInt32(right.value))));
                            case "==":
                                return new BoolValue(Convert.ToInt32(left.value) == Convert.ToInt32(right.value));
                            case "!=":
                                return new BoolValue(Convert.ToInt32(left.value) != Convert.ToInt32(right.value));
                            case "<":
                                return new BoolValue(Convert.ToInt32(left.value) < Convert.ToInt32(right.value));
                            case ">":
                                return new BoolValue(Convert.ToInt32(left.value) > Convert.ToInt32(right.value));
                            case "<=":
                                return new BoolValue(Convert.ToInt32(left.value) <= Convert.ToInt32(right.value));
                            case ">=":
                                return new BoolValue(Convert.ToInt32(left.value) >= Convert.ToInt32(right.value));

                            default:
                                operatorError();
                                break;
                        }
                        break;
                    case ValueTypes.Float: case ValueTypes.String: case ValueTypes.Null: case ValueTypes.Bool: case ValueTypes.Dictionary: case ValueTypes.Function: default:
                        typeError();
                        break;
                }
                break;
            case ValueTypes.Float:
                switch (right.type)
                {
                    case ValueTypes.Float: case ValueTypes.Integer:
                        switch (biexpr.opr)
                        {
                            case "+":
                                return new FloatValue(Convert.ToDouble(left.value) + Convert.ToDouble(right.value));
                            case "-":
                                return new FloatValue(Convert.ToDouble(left.value) - Convert.ToDouble(right.value));
                            case "*":
                                return new FloatValue(Convert.ToDouble(left.value) * Convert.ToDouble(right.value));
                            case "/":
                                if (Convert.ToDouble(left.value) == Convert.ToDouble(right.value))
                                    return new FloatValue(1);
                                else
                                    return new FloatValue(Convert.ToDouble(left.value) / Convert.ToDouble(right.value));
                            case "%":
                                return new FloatValue(Convert.ToDouble(left.value) % Convert.ToDouble(right.value));
                            case "^":
                                return new FloatValue(Math.Pow(Convert.ToDouble(left.value), Convert.ToDouble(right.value)));
                            case "==":
                                return new BoolValue(Convert.ToDouble(left.value) == Convert.ToDouble(right.value));
                            case "!=":
                                return new BoolValue(Convert.ToDouble(left.value) != Convert.ToDouble(right.value));
                            case "<":
                                return new BoolValue(Convert.ToDouble(left.value) < Convert.ToDouble(right.value));
                            case ">":
                                return new BoolValue(Convert.ToDouble(left.value) > Convert.ToDouble(right.value));
                            case "<=":
                                return new BoolValue(Convert.ToDouble(left.value) <= Convert.ToDouble(right.value));
                            case ">=":
                                return new BoolValue(Convert.ToDouble(left.value) >= Convert.ToDouble(right.value));

                            default:
                                operatorError();
                                break;
                        }
                        break;
                    case ValueTypes.String: case ValueTypes.Null: case ValueTypes.Bool: case ValueTypes.Dictionary: case ValueTypes.Function: default:
                        typeError();
                        break;
                }
                break;
            case ValueTypes.String:
                switch (right.type) {
                    case ValueTypes.String:
                        switch (biexpr.opr) {
                            case "+":
                                return new StringValue((string)left.value + (string)right.value);
                            case "==":
                                return new BoolValue((string)left.value == (string)right.value);
                            case "!=":
                                return new BoolValue((string)left.value != (string)right.value);

                            default:
                                operatorError();
                                break;
                        }
                        break;
                    case ValueTypes.Float: case ValueTypes.Integer: case ValueTypes.Null: case ValueTypes.Bool:
                        switch (biexpr.opr)
                        {
                            case "+":
                                return new StringValue(left.value + (string)right.value);

                            default:
                                operatorError();
                                break;
                        }
                        break;
                    case ValueTypes.Function: default:
                        typeError();
                        break;
                }
                break;
            case ValueTypes.Bool:
                switch (right.type) {
                    case ValueTypes.Bool:
                        switch (biexpr.opr) {
                            case "==":
                                return new BoolValue(left.value == right.value);
                            case "!=":
                                return new BoolValue(left.value != right.value);
                            case "&&":
                                return new BoolValue(Convert.ToBoolean(left.value) && Convert.ToBoolean(right.value));
                            case "||":
                                return new BoolValue(Convert.ToBoolean(left.value) || Convert.ToBoolean(right.value));

                            default:
                                operatorError();
                                break;
                        }
                        break;
                    case ValueTypes.Integer: case ValueTypes.Float: case ValueTypes.String: case ValueTypes.Null: case ValueTypes.Dictionary: case ValueTypes.Function: default:
                        typeError();
                        break;
                }
                break;

            default:
                typeError();
                return new StringValue("lol errors lol");
        }

        return new StringValue("lol unreachable lol");
    }

    public static StmtReturnValue EvaluateChunk(Chunk chunk, Environment env)
    {
        foreach (IStatement stmt in chunk.body)
        {
            if (stmt is IExpression expr)
                EvaluateExpr(expr, env);
            else
            {
                StmtReturnValue srv = EvaluateStmt(stmt, env);
                if (srv.hasValue)
                    return srv;
            }
        }
        return new();
    }

    public static IRuntimeValue EvaluateExpr(IExpression expr, Environment env)
    {
        switch (expr.Kind)
        {
            case StatementType.IntegerLit:
                return new IntegerValue() { value = ((IntegerLit)expr).value };
            case StatementType.FloatLit:
                return new FloatValue() { value = ((FloatLit)expr).value };
            case StatementType.StringLit:
                return new StringValue() { value = ((StringLit)expr).value };
            case StatementType.DictionaryLit: {
                DictionaryLit dict = (DictionaryLit)expr;
                Dictionary<IRuntimeValue, IRuntimeValue> props = new();
                foreach (DictionaryProperty prop in dict.props)
                {
                    if (prop.key.Kind == StatementType.Identifier)
                        props.Add(
                            new StringValue(((Identifier)prop.key).symbol),
                            EvaluateExpr(prop.value, env)
                        );
                    else
                        props.Add(
                            EvaluateExpr( prop.key , env),
                            EvaluateExpr(prop.value, env)
                        );
                }
                return new DictionaryValue() {
                    props = props
                };
            }
            case StatementType.ArrayLit:
                return new ArrayValue(((ArrayLit)expr).list.Select(elm => EvaluateExpr(elm, env)).ToList());
            case StatementType.LambdaLit: {
                LambdaLit lm = (LambdaLit)expr;
                List<IStatement> body = lm.body.body;
                return new FunctionValue((args) => {
                    if (args.Count != lm.parameters!.Count)
                        throw new($"Runtime Error:\n Function must take {lm.parameters.Count} arguments, got {args.Count} instead.");

                    Environment scope = new(env);
                    for (int i = 0; i < lm.parameters.Count; i++)
                        scope.DeclareValue(lm.parameters[i].symbol, args[i]);
                    foreach (IStatement stmt in body)
                    {
                        if (stmt is IExpression expr)
                            EvaluateExpr(expr, scope);
                        else
                        {
                            StmtReturnValue srv = EvaluateStmt(stmt, scope);
                            if (srv.hasValue)
                                return srv.value!;
                        }
                    }

                    return new NullValue(); // if there's no return statement, just return null
                });
            }

            case StatementType.Identifier:
                return env.GetValue(((Identifier)expr).symbol);

            case StatementType.BinaryExpr:
                return EvaluateBiExpr((BinaryExpr)expr, env);

            case StatementType.CallExpr: {
                CallExpr ce = (CallExpr)expr;
                List<IRuntimeValue> args = ce.args!.Select(arg => EvaluateExpr(arg, env)).ToList();
                FunctionValue fn = (FunctionValue)EvaluateExpr(ce.caller, env);

                if (fn.type != ValueTypes.Function)
                    throw new("Runtime Error:\n " + fn.value +
                    " is not a function, therefor it cannot be called. Expected: function Got: " + fn.type);

                return fn.call(args);
            }

            case StatementType.MemberExpr: {
                MemberExpr me = (MemberExpr)expr;
                IRuntimeValue obj = EvaluateExpr(me.obj, env);
                if (obj.type != ValueTypes.Dictionary && obj.type != ValueTypes.Array)
                    throw new("Runtime Error:\n Can't access member property of type: " + obj.type);

                if (obj.type == ValueTypes.Dictionary) // dictionaries
                {
                    DictionaryValue dv = (DictionaryValue)obj;
                    if (me.isComputed == true)
                    {
                        IRuntimeValue rt = new NullValue();
                        foreach (IRuntimeValue key in dv.props.Keys)
                            if (key.value == EvaluateExpr(me.property, env))
                                rt = dv.props[key];
                        return rt;
                    }
                    else
                    {
                        IRuntimeValue rt = new NullValue();
                        foreach (IRuntimeValue key in dv.props.Keys)
                            if (((StringValue)key).value == ((Identifier)me.property).symbol)
                                rt = dv.props[key];
                        return rt;
                    }
                }
                else // arrays
                {
                    ArrayValue av = (ArrayValue)obj;
                    if (me.isComputed == true)
                    {
                        IRuntimeValue i = EvaluateExpr(me.property, env);
                        if (i.type != ValueTypes.Integer)
                            throw new("Runtime Error:\n " + i.type +
                                " is not a valid array index type.");
                        int index = ((IntegerValue)i).value;

                        if (index > av.list.Count)
                            throw new(
                                "Runtime Error:\n Can't access array elements that have an index higher than the array's length.");
                        if (index < 0)
                            throw new("Runtime Error:\n Can't access array elements that have an index lower than 0.");
                        return av.list[index];
                    }
                    else
                    {
                        throw new("Runtime Error:\n Can't access non-computed properties on arrays.");
                    }
                }
            }

            default:
                throw new("Unknown Expression type, cannot evaluate as a value: " + expr);
        }
    }

    public static StmtReturnValue EvaluateStmt(IStatement stmt, Environment env)
    {
        switch (stmt.Kind)
        {
            case StatementType.Chunk:
                return EvaluateChunk((Chunk)stmt, env);

            case StatementType.ValDeclaration: {
                ValDeclaration vd = (ValDeclaration)stmt;
                IRuntimeValue value = EvaluateExpr(vd.value, env);
                env.DeclareValue(vd.identifier.symbol, value);
                return new();
            }

            case StatementType.FnDeclaration: {
                FnDeclaration fd = (FnDeclaration)stmt;
                List<IStatement> body = fd.body.body;
                env.DeclareValue(fd.identifier.symbol, new FunctionValue((args) => {
                    if (args.Count != fd.parameters!.Count)
                        throw new(
                            $"Runtime Error:\n Function must take {fd.parameters.Count} arguments, got {args.Count} arguments instead.");

                    Environment scope = new(env);
                    for (int i = 0; i < fd.parameters.Count; i++)
                        scope.DeclareValue(fd.parameters[i].symbol, args[i]);

                    foreach (IStatement stmt in body)
                    {
                        if (stmt is IExpression expr)
                            EvaluateExpr(expr, scope);
                        else
                        {
                            StmtReturnValue srv = EvaluateStmt(stmt, scope);
                            if (srv.hasValue)
                                return srv.value!;
                        }
                    }

                    return new NullValue(); // if there's no return statement, just return null
                }));
                return new(); // return null since it's a statement and not an expression
            }

            case StatementType.IfStmt: {
                IfStmt ifs = (IfStmt)stmt;
                if ((bool)EvaluateExpr(ifs.test, env).value)
                        return EvaluateChunk(ifs.consequent, new Environment(env));
                else if (ifs.alternate is not null)
                {
                    switch (ifs.alternate.Kind)
                    {
                        case StatementType.Chunk:
                            return EvaluateChunk((Chunk)ifs.alternate, new Environment(env));
                        case StatementType.IfStmt:
                            return EvaluateStmt(ifs.alternate, env);
                    }
                }

                return new(); // return null since it's a statement and not an expression
            }

            case StatementType.ReturnStmt:
                return new(EvaluateExpr(((ReturnStmt)stmt).value, env));

            default:
                throw new("Unknown Statement type, cannot execute: " + stmt);
        }
    }

    public sealed class StmtReturnValue
    {
        public readonly IRuntimeValue? value;
        public readonly bool hasValue;

        public StmtReturnValue(IRuntimeValue value)
        {
            this.value = value;
            hasValue = true;
        }

        public StmtReturnValue()
        {
            hasValue = false;
        }
    }
}
