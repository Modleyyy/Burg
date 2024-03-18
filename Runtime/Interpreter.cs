namespace Burg.Runtime;

using Burg.FrontEnd.AST;

public static class Interpreter
{
    public static IRuntimeValue EvaluateBiexpr(BinaryExpr biexpr, Environment env)
    {
        IRuntimeValue left = Evaluate(biexpr.left, env);
        IRuntimeValue right = Evaluate(biexpr.right, env);

        var typeError = () => {
            Console.WriteLine("Runtime Error:\n", "Binary Expression type error:");
            Console.WriteLine("\tLeft type: ", left.type);
            Console.WriteLine("\tRight type: ", right.type);
            throw new();
        };
        var operatorError = () => {
            Console.WriteLine("Runtime Error:\n", "Binary Expression operator error:");
            Console.WriteLine("\tOperator: ", biexpr.opr);
            Console.WriteLine("\tLeft type: ", left.type);
            Console.WriteLine("\tRight type: ", right.type);
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

    public static IRuntimeValue EvaluateChunk(Chunk chunk, Environment env)
    {
        foreach (IStatement node in chunk.body)
        {
            if (node.Kind == StatementType.ReturnStmt)
                return Evaluate(((ReturnStmt)node).value, env);
            Evaluate(node, env);
        }
        return new NullValue();
    }

    public static IRuntimeValue Evaluate(IStatement astNode, Environment env)
    {
        switch (astNode.Kind)
        {
            case StatementType.IntegerLit:
                return new IntegerValue() { value = ((IntegerLit)astNode).value };
            case StatementType.FloatLit:
                return new FloatValue() { value = ((FloatLit)astNode).value };
            case StatementType.StringLit:
                return new StringValue() { value = ((StringLit)astNode).value };
            case StatementType.DictionaryLit: {
                DictionaryLit dict = (DictionaryLit)astNode;
                Dictionary<IRuntimeValue, IRuntimeValue> props = new();
                foreach (DictionaryProperty prop in dict.props)
                {
                    if (prop.value.Kind == StatementType.Identifier)
                    {
                        props.Add(
                            new StringValue() { value = ((Identifier)prop.key).symbol },
                            Evaluate(prop.value, env)
                        );
                    }
                    else
                    {
                        props.Add(
                            Evaluate( prop.key , env),
                            Evaluate(prop.value, env)
                        );
                    }
                }
                return new DictionaryValue() {
                    props = props
                };
            }
            case StatementType.ArrayLit: {
                return new ArrayValue() {
                    list = ((ArrayLit)astNode).list.Select(elm => Evaluate(elm, env)).ToList()
                };
            }

            case StatementType.Chunk:
                return EvaluateChunk((Chunk)astNode, env);

            case StatementType.ValDeclaration: {
                ValDeclaration vd = (ValDeclaration)astNode;
                IRuntimeValue value = Evaluate(vd.value, env);
                env.DeclareValue(vd.identifier.symbol, value);
                return new NullValue();
            }

            case StatementType.FnDeclaration: {
                FnDeclaration fd = (FnDeclaration)astNode;
                List<IStatement> body = fd.body.body;
                env.DeclareValue(fd.identifier.symbol, new FunctionValue((args) => {
                    if (args.Count != fd.parameters!.Count)
                        throw new(
                            $"Runtime Error:\n Function must take {fd.parameters.Count} arguments, got {args.Count} arguments instead.");

                    Environment scope = new(env);
                    for (int i = 0; i < fd.parameters.Count; i++) {
                        scope.DeclareValue(fd.parameters[i].symbol, args[i]);
                    }
                    foreach (IStatement stmt in body) {
                        IRuntimeValue val = Evaluate(stmt, scope);
                        if (val.type == ValueTypes.Return)
                            return ((ReturnValue)val).value;
                    }

                    return new NullValue(); // if there's no return statement, just return null
                }));
                return new NullValue(); // return null since it's a statement and not an expression
            }

            case StatementType.LambdaLit: {
                LambdaLit lm = (LambdaLit)astNode;
                List<IStatement> body = lm.body.body;
                return new FunctionValue((args) => {
                    if (args.Count != lm.parameters!.Count)
                        throw new($"Runtime Error:\n Function must take {lm.parameters.Count} arguments, got {args.Count} instead.");

                    Environment scope = new(env);
                    for (int i = 0; i < lm.parameters.Count; i++)
                        scope.DeclareValue(lm.parameters[i].symbol, args[i]);
                    foreach (IStatement stmt in body) {
                        IRuntimeValue val = Evaluate(stmt, scope);
                        if (val.type == ValueTypes.Return)
                            return ((ReturnValue)val).value;
                    }

                    return new NullValue(); // if there's no return statement, just return null
                });
            }

            case StatementType.IfStmt: {
                IfStmt ifs = (IfStmt)astNode;
                if ((bool)Evaluate(ifs.test, env).value)
                        return EvaluateChunk(ifs.consequent, new Environment(env));
                else if (ifs.alternate is not null)
                {
                    switch (ifs.alternate.Kind)
                    {
                        case StatementType.Chunk:
                            return EvaluateChunk((Chunk)ifs.alternate, new Environment(env));
                        case StatementType.IfStmt:
                            return Evaluate(ifs.alternate, env);
                    }
                }

                return new NullValue(); // return null since it's a statement and not an expression
            }

            case StatementType.ReturnStmt:
                return new ReturnValue() { value = Evaluate(((ReturnStmt)astNode).value, env) };

            case StatementType.Identifier:
                return env.GetValue(((Identifier)astNode).symbol);

            case StatementType.BinaryExpr:
                return EvaluateBiexpr((BinaryExpr)astNode, env);

            case StatementType.CallExpr: {
                CallExpr ce = (CallExpr)astNode;
                List<IRuntimeValue> args = ce.args!.Select(arg => Evaluate(arg, env)).ToList();
                FunctionValue fn = (FunctionValue)Evaluate(ce.caller, env);

                if (fn.type != ValueTypes.Function)
                    throw new("Runtime Error:\n " + fn.value +
                    " is not a function, therefor it cannot be called. Expected: function Got: " + fn.type);

                return fn.call(args);
            }

            case StatementType.MemberExpr: {
                MemberExpr me = (MemberExpr)astNode;
                IRuntimeValue obj = Evaluate(me.obj, env);
                if (obj.type != ValueTypes.Dictionary && obj.type != ValueTypes.Array)
                    throw new("Runtime Error:\n Can't access member property of type: " + obj.type);

                if (obj.type == ValueTypes.Dictionary) // dictionaries
                {
                    DictionaryValue dv = (DictionaryValue)obj;
                    if (me.isComputed == true)
                    {
                        IRuntimeValue rt = new NullValue();
                        foreach (IRuntimeValue key in dv.props.Keys)
                            if (key.value == Evaluate(me.property, env))
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
                        IRuntimeValue i = Evaluate(me.property, env);
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
                throw new("Unknown Statement type, cannot evaluate as a value: " + astNode);
        }
    }
}
