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
            from statements in Parse.Ref(() => Statements.Statement!).Many().AtEnd()
            select (object)new AST.Module(
                statements.Cast<AST.Statements.Statement>().ToArray()
            );

        public static class Statements
        {
            public static TokenListParser<Tokens, object> VariableDeclaration { get; } =
                from var_keyword in Token.EqualTo(Tokens.Var)
                from name in Parse.Ref(() => Expressions.Name!)
                from colon in Token.EqualTo(Tokens.Colon)
                from type in Parse.Ref(() => Expressions.TypeIdentifier!)
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
                from type in Parse.Ref(() => Expressions.TypeIdentifier!)
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
                from name in Parse.Ref(() => Expressions.NameIdentifier!)
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
                    .Many()
                    .Between(Token.EqualTo(Tokens.LParen), Token.EqualTo(Tokens.RParen))
                from colon in Token.EqualTo(Tokens.Colon)
                from return_type in Parse.Ref(() => Expressions.TypeIdentifier!)
                from body in Parse.Ref(() => Statement!)
                    .Many()
                    .Between(Token.EqualTo(Tokens.LCurlyBracket), Token.EqualTo(Tokens.RCurlyBracket))
                select (object)new AST.Statements.FunctionDefinition(
                    (AST.Expressions.Name)name,
                    parameters.Cast<AST.Statements.AliasDeclaration>().ToArray(),
                    (AST.Expressions.TypeIdentifier)return_type,
                    body.Cast<AST.Statements.Statement>().ToArray()
                );

            public static TokenListParser<Tokens, object> If { get; } =
                from if_keyword in Token.EqualTo(Tokens.If)
                from condition in Parse.Ref(() => Expressions.Expression!)
                    .Between(Token.EqualTo(Tokens.LParen), Token.EqualTo(Tokens.RParen))
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

            public static TokenListParser<Tokens, object> Struct { get; } =
                from struct_keyword in Token.EqualTo(Tokens.Struct)
                from name in Parse.Ref(() => Expressions.Name!)
                from openCurly in Token.EqualTo(Tokens.LCurlyBracket)
                from constants in Parse.Ref(() => ConstantDeclaration!)
                    .Many()
                from variables in Parse.Ref(() => VariableDeclaration!)
                    .Many()
                from methods in Parse.Ref(() => FunctionDefinition!)
                    .Many()
                from closeCurly in Token.EqualTo(Tokens.RCurlyBracket)
                select (object)new AST.Statements.StructDefinition(
                    (AST.Expressions.Name)name,
                    constants.Cast<AST.Statements.ConstantDeclaration>().ToArray(),
                    variables.Cast<AST.Statements.VariableDeclaration>().ToArray(),
                    methods.Cast<AST.Statements.FunctionDefinition>().ToArray()
                );

            public static TokenListParser<Tokens, object> Statement { get; } =
                Parse.OneOf(
                    FunctionDefinition,
                    AliasDeclaration,
                    ReturnStatement,
                    Conditional,
                    While,
                    For,
                    Struct,
                    ValueAssignment.Try(),
                    SideEffectStatement
                );
        }

        public static class Expressions
        {
            public static TokenListParser<Tokens, object> Name { get; } =
                from name in Token.EqualTo(Tokens.Name)
                select (object)new AST.Expressions.Name(name.ToStringValue());

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

            public static TokenListParser<Tokens, object> DotAccess { get; } =
                from first in Parse.Ref(() => Name!)
                from dot in Token.EqualTo(Tokens.Dot)
                from second in Parse.Ref(() => NameIdentifier!)
                select (object)new AST.Expressions.DotAccess(
                    (AST.Expressions.NameIdentifier)first,
                    (AST.Expressions.NameIdentifier)second
                );

            public static TokenListParser<Tokens, object> NameIdentifier { get; } =
                Parse.OneOf(
                    Parse.Ref(() => Name!),
                    Parse.Ref(() => DotAccess!)
                );

            private static TextParser<string> InlineAssemblyParser { get; } =
                from asm in Span.EqualTo("asm")
                from whitespace in Character.WhiteSpace.Many()
                from openCurlyBrace in Character.EqualTo('{')
                from content in Span.Except("}")
                from closeCurlyBrace in Character.EqualTo('}')
                select string.Join(
                    "\n", content.ToStringValue()
                        .Split("\n")
                        .Select(s => s.Trim())
                        .Where(s => s.Length != 0)
                        .ToArray()
                );

            public static TokenListParser<Tokens, object> InlineAssembly { get; } =
                Token.EqualTo(Tokens.Asm)
                    .Apply(InlineAssemblyParser)
                    .Select(s => (object)new AST.Expressions.InlineAssembly(s));

            public static TokenListParser<Tokens, object> Cast { get; } =
                from type in Parse.Ref(() => TypeIdentifier!)
                    .Between(Token.EqualTo(Tokens.LAngleBracket), Token.EqualTo(Tokens.RAngleBracket))
                from expression in Parse.Ref(() => Expression!)
                select (object)new AST.Expressions.Cast(
                    (AST.Expressions.TypeIdentifier)type,
                    (AST.Expressions.Expression)expression
                );

            public static TokenListParser<Tokens, object> Expression { get; } =
            Parse.OneOf(
                Parse.Ref(() => StringLiteral!),
                Parse.Ref(() => IntegerLiteral!),
                Parse.Ref(() => InlineAssembly!),
                Parse.Ref(() => Cast!),
                Parse.Ref(() => FunctionCall!.Try()),
                Parse.Ref(() => NameIdentifier!)
            );

            public static TokenListParser<Tokens, object> TypeName { get; } =
                from type_name in Token.EqualTo(Tokens.Name)
                select (object)new AST.Expressions.TypeName(type_name.ToStringValue());

            public static TokenListParser<Tokens, object> GenericTypeName { get; } =
                from type_name in Token.EqualTo(Tokens.GenericName)
                select (object)new AST.Expressions.GenericTypeName(type_name.ToStringValue());

            public static TokenListParser<Tokens, object> TypeNameLike { get; } =
                Parse.OneOf(
                    Parse.Ref(() => TypeName!),
                    Parse.Ref(() => GenericTypeName!)
                );

            public static TokenListParser<Tokens, object> TypeDotAccess { get; } =
                from first in Parse.Ref(() => TypeNameLike!)
                from dot in Token.EqualTo(Tokens.Dot)
                from second in Parse.Ref(() => TypeIdentifier!)
                select (object)new AST.Expressions.TypeDotAccess(
                    (AST.Expressions.TypeIdentifier)first,
                    (AST.Expressions.TypeIdentifier)second
                );

            public static TokenListParser<Tokens, object> GenericTypeSpecialization { get; } =
                from identifier in Parse.Ref(() => TypeNameLike!)
                from specialization in Parse.Ref(() => TypeIdentifier!)
                    .ManyDelimitedBy(Token.EqualTo(Tokens.Comma))
                    .Between(Token.EqualTo(Tokens.LAngleBracket), Token.EqualTo(Tokens.RAngleBracket))
                select (object)new AST.Expressions.GenericTypeSpecialization(
                    (AST.Expressions.TypeIdentifier)identifier,
                    specialization.Cast<AST.Expressions.TypeIdentifier>().ToArray()
                );

            public static TokenListParser<Tokens, object> PointerType { get; } =
                from identifier in Parse.Ref(() => TypeNameLike!)
                from pointer in Token.EqualTo(Tokens.Asterisk)
                select (object)new AST.Expressions.PointerType(
                    (AST.Expressions.TypeIdentifier)identifier
                );

            public static TokenListParser<Tokens, object> TypeIdentifier { get; } =
                Parse.OneOf(
                    Parse.Ref(() => GenericTypeSpecialization!).Try(),
                    Parse.Ref(() => TypeDotAccess!).Try(),
                    Parse.Ref(() => PointerType!).Try(),
                    Parse.Ref(() => TypeNameLike!)
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