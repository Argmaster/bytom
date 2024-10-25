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
            from statements in Parse.Ref(() => Statement!).Many()
            select (object)new AST.Module(
                statements.Cast<AST.Statement>().ToArray()
            );

        public static TokenListParser<Tokens, object> AliasDeclarationStatement { get; } =
            Parse.OneOf(
                Parse.Ref(() => VariableDeclaration!),
                Parse.Ref(() => ConstantDeclaration!)
            );

        public static TokenListParser<Tokens, object> Statement { get; } =
            Parse.OneOf(
                Parse.Ref(() => FunctionDefinition!),
                Parse.Ref(() => AliasDeclarationStatement!),
                Parse.Ref(() => ReturnStatement!),
                Parse.Ref(() => SideEffectStatement!)
            );

        public static TokenListParser<Tokens, object> Expression { get; } =
            Parse.OneOf(
                Parse.Ref(() => StringLiteral!),
                Parse.Ref(() => FunctionCallExpression!),
                Parse.Ref(() => Name!)
            );

        public static TokenListParser<Tokens, object> ReturnStatement { get; } =
            from return_keyword in Token.EqualTo(Tokens.Return)
            from value in Parse.Ref(() => Expression!)
            from semicolon in Token.EqualTo(Tokens.Semicolon)
            select (object)new AST.Return(
                (AST.Expression)value
            );

        public static TokenListParser<Tokens, object> FunctionCallExpression { get; } =
            from name in Parse.Ref(() => Name!)
            from openParen in Token.EqualTo(Tokens.LParen)
            from arguments in Parse.Ref(() => Name!)
                .ManyDelimitedBy(
                    delimiter: Token.EqualTo(Tokens.Comma),
                    end: Token.EqualTo(Tokens.RParen)
                )
            select (object)new AST.FunctionCall(
                (AST.Name)name,
                arguments.Cast<AST.Expression>().ToArray()
            );

        public static TokenListParser<Tokens, object> SideEffectStatement { get; } =
            from expression in Parse.Ref(() => Expression!)
            from semicolon in Token.EqualTo(Tokens.Semicolon)
            select (object)new AST.SideEffect(
                (AST.Expression)expression
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
                .Select(s => (object)new AST.StringLiteral(s));

        public static TokenListParser<Tokens, object> Name { get; } =
            Token.EqualTo(Tokens.Name)
                .Select(s => (object)new AST.Name(s.ToStringValue()));

        public static TokenListParser<Tokens, object> TypeName { get; } =
            from name in Token.EqualTo(Tokens.Name)
            from pointer in Token.EqualTo(Tokens.Asterisk).Many()
            select (object)new AST.TypeName(name.ToStringValue(), pointer.Length);

        public static TokenListParser<Tokens, object> VariableDeclaration { get; } =
            from var_keyword in Token.EqualTo(Tokens.Var)
            from name in Parse.Ref(() => Name!)
            from colon in Token.EqualTo(Tokens.Colon)
            from type in Parse.Ref(() => TypeName!)
            from semicolon in Token.EqualTo(Tokens.Semicolon)
            from value in Parse.Ref(() => Expression!).AsNullable()
            select (object)new AST.VariableDeclaration(
                (AST.Name)name,
                (AST.TypeName)type,
                (AST.Expression?)value
            );

        public static TokenListParser<Tokens, object> ConstantDeclaration { get; } =
            from const_keyword in Token.EqualTo(Tokens.Var)
            from name in Parse.Ref(() => Name!)
            from colon in Token.EqualTo(Tokens.Colon)
            from type in Parse.Ref(() => TypeName!)
            from semicolon in Token.EqualTo(Tokens.Semicolon)
            from value in Parse.Ref(() => Expression!)
            select (object)new AST.ConstantDeclaration(
                (AST.Name)name,
                (AST.TypeName)type,
                (AST.Expression)value
            );

        public static TokenListParser<Tokens, object> FunctionParameter { get; } =
            from name in Parse.Ref(() => Name!)
            from colon in Token.EqualTo(Tokens.Colon)
            from type in Parse.Ref(() => TypeName!)
            select (object)new AST.FunctionParameter(
                (AST.Name)name,
                (AST.TypeName)type
            );

        public static TokenListParser<Tokens, object> FunctionDefinition { get; } =
            from function in Token.EqualTo(Tokens.Function)
            from name in Parse.Ref(() => Name!)
            from openParen in Token.EqualTo(Tokens.LParen)
            from parameters in FunctionParameter.ManyDelimitedBy(Token.EqualTo(Tokens.Comma))
            from closeParen in Token.EqualTo(Tokens.RParen)
            from colon in Token.EqualTo(Tokens.Colon)
            from return_type in Parse.Ref(() => TypeName!)
            from openBrace in Token.EqualTo(Tokens.LCurlyBracket)
            from body in Parse.Ref(() => Statement!).Many()
            from closeBrace in Token.EqualTo(Tokens.RCurlyBracket)
            select (object)new AST.FunctionDefinition(
                (AST.Name)name,
                parameters.Cast<AST.FunctionParameter>().ToArray(),
                (AST.TypeName)return_type,
                body.Cast<AST.Statement>().ToArray()
            );

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