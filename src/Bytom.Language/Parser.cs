using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Superpower;
using Superpower.Display;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Bytom.Language
{
    public static class Parser
    {
        public static TokenListParser<Tokens, object> Module { get; } =
            from statements in Parse.Ref(() => Statements.Statement!).Many()
            select (object)new AST.Module(
                statements.Cast<AST.Statements.Statement>().ToArray()
            );

        public static class Statements
        {
            public static TokenListParser<Tokens, object> VariableDeclaration { get; } =
                from var_keyword in Token.EqualTo(Tokens.Var)
                from name in Parse.Ref(() => Expressions.Name!)
                from colon in Token.EqualTo(Tokens.Colon)
                from type in Parse.Ref(() => Expressions.TypeName!)
                from value in Parse.Ref(
                    () => Token.EqualTo(Tokens.Assignment)
                            .Then(_ => Expressions.Expression!)
                    ).AsNullable().OptionalOrDefault()
                from semicolon in Token.EqualTo(Tokens.Semicolon)
                select (object)new AST.Statements.VariableDeclaration(
                    (AST.Expressions.Name)name,
                    (AST.Expressions.TypeName)type,
                    (AST.Expressions.Expression?)value
                );

            public static TokenListParser<Tokens, object> ConstantDeclaration { get; } =
                from const_keyword in Token.EqualTo(Tokens.Const)
                from name in Parse.Ref(() => Expressions.Name!)
                from colon in Token.EqualTo(Tokens.Colon)
                from type in Parse.Ref(() => Expressions.TypeName!)
                from assignment in Token.EqualTo(Tokens.Assignment)
                from value in Parse.Ref(() => Expressions.Expression!)
                from semicolon in Token.EqualTo(Tokens.Semicolon)
                select (object)new AST.Statements.ConstantDeclaration(
                    (AST.Expressions.Name)name,
                    (AST.Expressions.TypeName)type,
                    (AST.Expressions.Expression)value
                );

            public static TokenListParser<Tokens, object> AliasDeclaration { get; } =
                Parse.OneOf(
                    VariableDeclaration.Try(),
                    ConstantDeclaration
                );

            public static TokenListParser<Tokens, object> ValueAssignment { get; } =
                from name in Parse.Ref(() => Expressions.Name!)
                from assignment in Token.EqualTo(Tokens.Assignment)
                from value in Parse.Ref(() => Expressions.Expression!)
                from semicolon in Token.EqualTo(Tokens.Semicolon)
                select (object)new AST.Statements.ValueAssignment(
                    (AST.Expressions.Name)name,
                    (AST.Expressions.Expression)value
                );

            public static TokenListParser<Tokens, object> ReturnStatement { get; } =
                from return_keyword in Token.EqualTo(Tokens.Return)
                from value in Parse.Ref(() => Expressions.Expression!)
                from semicolon in Token.EqualTo(Tokens.Semicolon)
                select (object)new AST.Statements.Return(
                    (AST.Expressions.Expression)value
                );

            public static TokenListParser<Tokens, object> SideEffectStatement { get; } =
                from expression in Parse.Ref(() => Expressions.Expression!)
                from semicolon in Token.EqualTo(Tokens.Semicolon)
                select (object)new AST.Statements.SideEffect(
                    (AST.Expressions.Expression)expression
                );

            public static TokenListParser<Tokens, object> FunctionDefinition { get; } =
                from function in Token.EqualTo(Tokens.Function)
                from name in Parse.Ref(() => Expressions.Name!)
                from parameters in Parse.Ref(() => AliasDeclaration!)
                    .ManyDelimitedBy(Token.EqualTo(Tokens.Comma))
                    .Between(Token.EqualTo(Tokens.LParen), Token.EqualTo(Tokens.RParen))
                from colon in Token.EqualTo(Tokens.Colon)
                from return_type in Parse.Ref(() => Expressions.TypeName!)
                from body in Parse.Ref(() => Statement!)
                    .Many()
                    .Between(Token.EqualTo(Tokens.LCurlyBracket), Token.EqualTo(Tokens.RCurlyBracket))
                select (object)new AST.Statements.FunctionDefinition(
                    (AST.Expressions.Name)name,
                    parameters.Cast<AST.Statements.AliasDeclaration>().ToArray(),
                    (AST.Expressions.TypeName)return_type,
                    body.Cast<AST.Statements.Statement>().ToArray()
                );

            public static TokenListParser<Tokens, object> If { get; } =
                from if_keyword in Token.EqualTo(Tokens.If)
                from openParen in Token.EqualTo(Tokens.LParen)
                from condition in Parse.Ref(() => Expressions.Expression!)
                from closeParen in Token.EqualTo(Tokens.RParen)
                from body in Parse.Ref(() => Statement!)
                    .Many()
                    .Between(Token.EqualTo(Tokens.LCurlyBracket), Token.EqualTo(Tokens.RCurlyBracket))
                select (object)new AST.Statements.If(
                    (AST.Expressions.Expression)condition,
                    body.Cast<AST.Statements.Statement>().ToArray()
                );

            public static TokenListParser<Tokens, object> Elif { get; } =
                from if_keyword in Token.EqualTo(Tokens.Elif)
                from condition in Parse.Ref(() => Expressions.Expression!)
                    .Between(Token.EqualTo(Tokens.LParen), Token.EqualTo(Tokens.RParen))
                from body in Parse.Ref(() => Statement!)
                    .Many()
                    .Between(Token.EqualTo(Tokens.LCurlyBracket), Token.EqualTo(Tokens.RCurlyBracket))
                select (object)new AST.Statements.If(
                    (AST.Expressions.Expression)condition,
                    body.Cast<AST.Statements.Statement>().ToArray()
                );

            public static TokenListParser<Tokens, object> Else { get; } =
                from else_keyword in Token.EqualTo(Tokens.Else)
                from body in Parse.Ref(() => Statement!)
                    .Many()
                    .Between(Token.EqualTo(Tokens.LCurlyBracket), Token.EqualTo(Tokens.RCurlyBracket))
                select (object)new AST.Statements.Else(
                    body.Cast<AST.Statements.Statement>().ToArray()
                );

            public static TokenListParser<Tokens, object> Conditional { get; } =
                from if_ in If
                from elif_ in Elif.Many()
                from else_ in Else.AsNullable().OptionalOrDefault()
                select (object)new AST.Statements.Conditional(
                    (AST.Statements.If)if_,
                    elif_.Cast<AST.Statements.If>().ToArray(),
                    (AST.Statements.Else?)else_
                );

            public static TokenListParser<Tokens, object> While { get; } =
                from while_keyword in Token.EqualTo(Tokens.While)
                from condition in Parse.Ref(() => Expressions.Expression!)
                    .Between(Token.EqualTo(Tokens.LParen), Token.EqualTo(Tokens.RParen))
                from body in Parse.Ref(() => Statement!)
                    .Many()
                    .Between(Token.EqualTo(Tokens.LCurlyBracket), Token.EqualTo(Tokens.RCurlyBracket))
                select (object)new AST.Statements.While(
                    (AST.Expressions.Expression)condition,
                    body.Cast<AST.Statements.Statement>().ToArray()
                );

            public static TokenListParser<Tokens, object> For { get; } =
                from for_keyword in Token.EqualTo(Tokens.For)
                from openParen in Token.EqualTo(Tokens.LParen)
                from initialization in Parse.Ref(() => VariableDeclaration!)
                from condition in Parse.Ref(() => Expressions.Expression!)
                from semicolon1 in Token.EqualTo(Tokens.Semicolon)
                from increment in Parse.Ref(() => ValueAssignment!)
                from closeParen in Token.EqualTo(Tokens.RParen)
                from body in Parse.Ref(() => Statement!)
                    .Many()
                    .Between(Token.EqualTo(Tokens.LCurlyBracket), Token.EqualTo(Tokens.RCurlyBracket))
                select (object)new AST.Statements.For(
                    (AST.Statements.VariableDeclaration)initialization,
                    (AST.Expressions.Expression)condition,
                    (AST.Statements.ValueAssignment)increment,
                    body.Cast<AST.Statements.Statement>().ToArray()
                );

            public static TokenListParser<Tokens, object> Statement { get; } =
                Parse.OneOf(
                    FunctionDefinition.Try(),
                    AliasDeclaration.Try(),
                    ReturnStatement.Try(),
                    Conditional.Try(),
                    While.Try(),
                    For.Try(),
                    ValueAssignment.Try(),
                    SideEffectStatement
                );
        }

        public static class Expressions
        {
            public static TokenListParser<Tokens, object> FunctionCall { get; } =
                from name in Parse.Ref(() => Name!)
                from arguments in Parse.Ref(() => Expression!)
                    .ManyDelimitedBy(Token.EqualTo(Tokens.Comma))
                    .Between(Token.EqualTo(Tokens.LParen), Token.EqualTo(Tokens.RParen))
                select (object)new AST.Expressions.FunctionCall(
                    (AST.Expressions.Name)name,
                    arguments.Cast<AST.Expressions.Expression>().ToArray()
                );


            private static TextParser<string> StringTextParser { get; } =
                from open in Character.EqualTo('"')
                from chars in Character.ExceptIn('"', '\\')
                    .Or(Character.EqualTo('\\')
                        .IgnoreThen(
                            Character.EqualTo('\\')
                            .Or(Character.EqualTo('"'))
                            .Or(Character.EqualTo('/'))
                            .Or(Character.EqualTo('b').Value('\b'))
                            .Or(Character.EqualTo('f').Value('\f'))
                            .Or(Character.EqualTo('n').Value('\n'))
                            .Or(Character.EqualTo('r').Value('\r'))
                            .Or(Character.EqualTo('t').Value('\t'))
                            .Or(Character.EqualTo('u').IgnoreThen(
                                    Span.MatchedBy(Character.HexDigit.Repeat(4))
                                        .Apply(Numerics.HexDigitsUInt32)
                                        .Select(cc => (char)cc)))
                            .Named("escape sequence")))
                    .Many()
                from close in Character.EqualTo('"')
                select new string(chars);

            public static TokenListParser<Tokens, object> StringLiteral { get; } =
                Token.EqualTo(Tokens.StringLiteral)
                    .Apply(StringTextParser)
                    .Select(s => (object)new AST.Expressions.StringLiteral(s));

            public static TokenListParser<Tokens, object> IntegerLiteral { get; } =
                Token.EqualTo(Tokens.IntegerLiteral)
                    .Apply(Numerics.IntegerInt64)
                    .Select(s => (object)new AST.Expressions.IntegerLiteral(s));

            public static TokenListParser<Tokens, object> Name { get; } =
                Token.EqualTo(Tokens.Name)
                    .Select(s => (object)new AST.Expressions.Name(s.ToStringValue()));

            public static TokenListParser<Tokens, object> LeftIdentifier { get; } =
                Parse.OneOf(
                    Parse.Ref(() => Name!)
                );

            public static TokenListParser<Tokens, object> TypeName { get; } =
                from name in Token.EqualTo(Tokens.Name)
                from pointer in Token.EqualTo(Tokens.Asterisk).Many()
                select (object)new AST.Expressions.TypeName(name.ToStringValue(), pointer.Length);

            public static TokenListParser<Tokens, object> Expression { get; } =
            Parse.OneOf(
                StringLiteral.Try(),
                IntegerLiteral.Try(),
                FunctionCall.Try(),
                LeftIdentifier
            );
        }

        public static bool TryParse(
            string json,
            out object? value,
            [MaybeNullWhen(true)] out string error,
            out Position errorPosition
        )
        {
            var tokens = Tokenizer.Instance.TryTokenize(json);
            if (!tokens.HasValue)
            {
                value = null;
                error = tokens.ToString();
                errorPosition = tokens.ErrorPosition;
                return false;
            }

            var parsed = Module.TryParse(tokens.Value);
            if (!parsed.HasValue)
            {
                value = null;
                error = parsed.ToString();
                errorPosition = parsed.ErrorPosition;
                return false;
            }

            value = parsed.Value;
            error = null;
            errorPosition = Position.Empty;
            return true;
        }
    }
}