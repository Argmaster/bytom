namespace Bytom.Language.Tests;

public class ParserTests
{
    [SetUp]
    public void Setup()
    {
    }
    public class FunctionDefinitionTests
    {
        [Test]
        public void TestSquare()
        {
            bool result = Parser.TryParse(@"
            function square(var x: i32;): i32
            {
                return mul(x, x);
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("square"));
            Assert.That(function!.arguments, Has.Length.EqualTo(1));

            var argument = (AST.Statements.VariableDeclaration)function!.arguments[0];
            Assert.That(argument!.name.name, Is.EqualTo("x"));
            Assert.That(argument!.type.ToString(), Is.EqualTo("i32"));
            Assert.That(argument!.type.GetPointerLevel(), Is.EqualTo(0));

            Assert.That(function!.return_type.ToString(), Is.EqualTo("i32"));
            Assert.That(function!.return_type.GetPointerLevel(), Is.EqualTo(0));

            Assert.That(function!.body, Has.Length.EqualTo(1));
            Assert.That(function!.body[0], Is.InstanceOf<AST.Statements.Return>());

            var returnStatement = (AST.Statements.Return)function!.body[0];
            Assert.That(returnStatement!.value, Is.InstanceOf<AST.Expressions.FunctionCall>());

            var functionCall = (AST.Expressions.FunctionCall)returnStatement!.value;
            Assert.That(functionCall!.name.name, Is.EqualTo("mul"));
            Assert.That(functionCall!.arguments, Has.Length.EqualTo(2));

            Assert.That(functionCall!.arguments[0], Is.InstanceOf<AST.Expressions.Name>());
            var argument1 = (AST.Expressions.Name)functionCall!.arguments[0];
            Assert.That(argument1!.name, Is.EqualTo("x"));

            Assert.That(functionCall!.arguments[1], Is.InstanceOf<AST.Expressions.Name>());
            var argument2 = (AST.Expressions.Name)functionCall!.arguments[1];
            Assert.That(argument2!.name, Is.EqualTo("x"));
        }
        [Test]
        public void TestTwoParams()
        {
            bool result = Parser.TryParse(@"
            function do_something(var x: i32; var y: i32;): i32
            {
                const j: i32 = add(x, y);
                var result: i32 = 0;

                for (var i: i32 = 0; lt(i, 10); i = inc(i);)
                {
                    result = add(j, 5);
                }
                return mul(result, result);
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("do_something"));
            Assert.That(function!.arguments, Has.Length.EqualTo(2));

            var argument = (AST.Statements.VariableDeclaration)function!.arguments[0];
            Assert.That(argument!.name.name, Is.EqualTo("x"));
            Assert.That(argument!.type.ToString(), Is.EqualTo("i32"));
            Assert.That(argument!.type.GetPointerLevel(), Is.EqualTo(0));

            var argument2 = (AST.Statements.VariableDeclaration)function!.arguments[1];
            Assert.That(argument2!.name.name, Is.EqualTo("y"));
            Assert.That(argument2!.type.ToString(), Is.EqualTo("i32"));
            Assert.That(argument2!.type.GetPointerLevel(), Is.EqualTo(0));

            Assert.That(function!.return_type.ToString(), Is.EqualTo("i32"));
            Assert.That(function!.return_type.GetPointerLevel(), Is.EqualTo(0));

            Assert.That(function!.body, Has.Length.EqualTo(4));
            Assert.That(function!.body[0], Is.InstanceOf<AST.Statements.ConstantDeclaration>());
            Assert.That(function!.body[1], Is.InstanceOf<AST.Statements.VariableDeclaration>());
            Assert.That(function!.body[2], Is.InstanceOf<AST.Statements.For>());
            Assert.That(function!.body[3], Is.InstanceOf<AST.Statements.Return>());
        }
    }

    public class SideEffectTests
    {
        [Test]
        public void TestFunctionCallIntInt()
        {
            bool result = Parser.TryParse(@"
            mul(1, 1);
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());
        }

        [Test]
        public void TestFunctionCallIntVar()
        {
            bool result = Parser.TryParse(@"
            mul(1, x);
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());
        }

        [Test]
        public void TestFunctionCallVarInt()
        {
            bool result = Parser.TryParse(@"
            mul(x, 1);
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());
        }

        [Test]
        public void TestFunctionCallVarVar()
        {
            bool result = Parser.TryParse(@"
            mul(x, 1);
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());
        }
    }

    [Test]
    public void TestVariableDeclaration()
    {
        bool result = Parser.TryParse(@"
            function main(): void
            {
                var y: i32;
                var x: i32 = 1;
            }
            ",
            out var value,
            out var error,
            out var errorPosition
        );
        Assert.That(error, Is.Null);
        Assert.That(result, Is.True);
        Assert.That(value, Is.InstanceOf<AST.Module>());

        var module = (AST.Module)value;
        Assert.That(module!.statements, Has.Length.EqualTo(1));

        var statement = module!.statements[0];
        Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

        var function = (AST.Statements.FunctionDefinition)statement;
        Assert.That(function!.name.name, Is.EqualTo("main"));
        Assert.That(function!.arguments, Has.Length.EqualTo(0));
        Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
        Assert.That(function!.body, Has.Length.EqualTo(2));

        Assert.That(function!.body[0], Is.InstanceOf<AST.Statements.VariableDeclaration>());
        var variableDeclaration1 = (AST.Statements.VariableDeclaration)function!.body[0];
        Assert.That(variableDeclaration1!.name.name, Is.EqualTo("y"));
        Assert.That(variableDeclaration1!.type.ToString(), Is.EqualTo("i32"));
        Assert.That(variableDeclaration1!.value, Is.Null);

        Assert.That(function!.body[1], Is.InstanceOf<AST.Statements.VariableDeclaration>());
        var variableDeclaration2 = (AST.Statements.VariableDeclaration)function!.body[1];
        Assert.That(variableDeclaration2!.name.name, Is.EqualTo("x"));
        Assert.That(variableDeclaration2!.type.ToString(), Is.EqualTo("i32"));
        Assert.That(variableDeclaration2!.value, Is.InstanceOf<AST.Expressions.IntegerLiteral>());
    }

    [Test]
    public void TestConstantDeclaration()
    {
        bool result = Parser.TryParse(@"
            function main(): void
            {
                const x: i32 = 1;
            }
            ",
            out var value,
            out var error,
            out var errorPosition
        );
        Assert.That(error, Is.Null);
        Assert.That(result, Is.True);
        Assert.That(value, Is.InstanceOf<AST.Module>());

        var module = (AST.Module)value;
        Assert.That(module!.statements, Has.Length.EqualTo(1));

        var statement = module!.statements[0];
        Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

        var function = (AST.Statements.FunctionDefinition)statement;
        Assert.That(function!.name.name, Is.EqualTo("main"));
        Assert.That(function!.arguments, Has.Length.EqualTo(0));
        Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
        Assert.That(function!.body, Has.Length.EqualTo(1));

        Assert.That(function!.body[0], Is.InstanceOf<AST.Statements.ConstantDeclaration>());
        var variableDeclaration2 = (AST.Statements.ConstantDeclaration)function!.body[0];
        Assert.That(variableDeclaration2!.name.name, Is.EqualTo("x"));
        Assert.That(variableDeclaration2!.type.ToString(), Is.EqualTo("i32"));
        Assert.That(variableDeclaration2!.value, Is.InstanceOf<AST.Expressions.IntegerLiteral>());
    }

    [Test]
    public void TestValueAssignment()
    {
        bool result = Parser.TryParse(@"
            function main(): void
            {
                var x: i32;
                x = 1;
            }
            ",
            out var value,
            out var error,
            out var errorPosition
        );
        Assert.That(error, Is.Null);
        Assert.That(result, Is.True);
        Assert.That(value, Is.InstanceOf<AST.Module>());

        var module = (AST.Module)value;
        Assert.That(module!.statements, Has.Length.EqualTo(1));

        var statement = module!.statements[0];
        Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

        var function = (AST.Statements.FunctionDefinition)statement;
        Assert.That(function!.name.name, Is.EqualTo("main"));
        Assert.That(function!.arguments, Has.Length.EqualTo(0));
        Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
        Assert.That(function!.body, Has.Length.EqualTo(2));

        Assert.That(function!.body[1], Is.InstanceOf<AST.Statements.ValueAssignment>());
        var valueAssignment = (AST.Statements.ValueAssignment)function!.body[1];

        Assert.That(valueAssignment!.name, Is.InstanceOf<AST.Expressions.Name>());
        var name = (AST.Expressions.Name)valueAssignment!.name;
        Assert.That(name!.name, Is.EqualTo("x"));

        Assert.That(valueAssignment!.value, Is.InstanceOf<AST.Expressions.IntegerLiteral>());
        var val = (AST.Expressions.IntegerLiteral)valueAssignment!.value;
        Assert.That(val!.value, Is.EqualTo(1));
    }

    public class IfElifElseTests
    {
        [Test]
        public void TestIfElifElse()
        {
            bool result = Parser.TryParse(@"
            function main(): void
            {
                var x: i32 = 1;
                if (eq(x, 1))
                {
                    x = 2;
                }
                elif (eq(x, 2))
                {
                    x = 3;
                }
                else
                {
                    x = 4;
                }
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("main"));
            Assert.That(function!.arguments, Has.Length.EqualTo(0));
            Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
            Assert.That(function!.body, Has.Length.EqualTo(2));

            var conditionalStatement = (AST.Statements.Conditional)function!.body[1];
            Assert.That(conditionalStatement!.if_, Is.InstanceOf<AST.Statements.If>());
            Assert.That(conditionalStatement!.elif_, Has.Length.EqualTo(1));
            Assert.That(conditionalStatement!.else_, Is.InstanceOf<AST.Statements.Else>());

            var ifStatement = conditionalStatement!.if_;
            Assert.That(ifStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(ifStatement!.body, Has.Length.EqualTo(1));

            var elifStatement = conditionalStatement!.elif_[0];
            Assert.That(elifStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(elifStatement!.body, Has.Length.EqualTo(1));

            var elseStatement = conditionalStatement!.else_;
            Assert.That(elseStatement!.body, Has.Length.EqualTo(1));
        }

        [Test]
        public void TestIfElif()
        {
            bool result = Parser.TryParse(@"
            function main(): void
            {
                var x: i32 = 1;
                if (eq(x, 1))
                {
                    x = 2;
                }
                elif (eq(x, 2))
                {
                    x = 3;
                }
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("main"));
            Assert.That(function!.arguments, Has.Length.EqualTo(0));
            Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
            Assert.That(function!.body, Has.Length.EqualTo(2));

            var conditionalStatement = (AST.Statements.Conditional)function!.body[1];
            Assert.That(conditionalStatement!.if_, Is.InstanceOf<AST.Statements.If>());
            Assert.That(conditionalStatement!.elif_, Has.Length.EqualTo(1));
            Assert.That(conditionalStatement!.else_, Is.Null);

            var ifStatement = conditionalStatement!.if_;
            Assert.That(ifStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(ifStatement!.body, Has.Length.EqualTo(1));

            var elifStatement = conditionalStatement!.elif_[0];
            Assert.That(elifStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(elifStatement!.body, Has.Length.EqualTo(1));
        }

        [Test]
        public void TestIf()
        {
            bool result = Parser.TryParse(@"
            function main(): void
            {
                var x: i32 = 1;
                if (eq(x, 1))
                {
                    x = 2;
                }
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("main"));
            Assert.That(function!.arguments, Has.Length.EqualTo(0));
            Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
            Assert.That(function!.body, Has.Length.EqualTo(2));

            var conditionalStatement = (AST.Statements.Conditional)function!.body[1];
            Assert.That(conditionalStatement!.if_, Is.InstanceOf<AST.Statements.If>());
            Assert.That(conditionalStatement!.elif_, Has.Length.EqualTo(0));
            Assert.That(conditionalStatement!.else_, Is.Null);

            var ifStatement = conditionalStatement!.if_;
            Assert.That(ifStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(ifStatement!.body, Has.Length.EqualTo(1));
        }

        [Test]
        public void TestIfElifElif()
        {
            bool result = Parser.TryParse(@"
            function main(): void
            {
                var x: i32 = 1;
                if (eq(x, 1))
                {
                    x = 2;
                }
                elif (eq(x, 2))
                {
                    x = 3;
                }
                elif (eq(x, 3))
                {
                    x = 4;
                }
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("main"));
            Assert.That(function!.arguments, Has.Length.EqualTo(0));
            Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
            Assert.That(function!.body, Has.Length.EqualTo(2));

            var conditionalStatement = (AST.Statements.Conditional)function!.body[1];
            Assert.That(conditionalStatement!.if_, Is.InstanceOf<AST.Statements.If>());
            Assert.That(conditionalStatement!.elif_, Has.Length.EqualTo(2));
            Assert.That(conditionalStatement!.else_, Is.Null);

            var ifStatement = conditionalStatement!.if_;
            Assert.That(ifStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(ifStatement!.body, Has.Length.EqualTo(1));

            var elifStatement = conditionalStatement!.elif_[0];
            Assert.That(elifStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(elifStatement!.body, Has.Length.EqualTo(1));

            var elifStatement2 = conditionalStatement!.elif_[1];
            Assert.That(elifStatement2!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(elifStatement2!.body, Has.Length.EqualTo(1));
        }
    }

    public class LoopTests
    {
        [Test]
        public void TestWhile()
        {
            bool result = Parser.TryParse(@"
            function main(): void
            {
                var x: i32 = 1;
                while (eq(x, 1))
                {
                    x = 2;
                }
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("main"));
            Assert.That(function!.arguments, Has.Length.EqualTo(0));
            Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
            Assert.That(function!.body, Has.Length.EqualTo(2));

            var whileStatement = (AST.Statements.While)function!.body[1];
            Assert.That(whileStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(whileStatement!.body, Has.Length.EqualTo(1));
        }
        [Test]
        public void TestFor()
        {
            bool result = Parser.TryParse(@"
            function main(): void
            {
                var x: i32 = 1;
                for (var i: i32 = 0; lt(i, 10); i = inc(i);)
                {
                    x = add(x, 2);
                }
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("main"));
            Assert.That(function!.arguments, Has.Length.EqualTo(0));
            Assert.That(function!.return_type.ToString(), Is.EqualTo("void"));
            Assert.That(function!.body, Has.Length.EqualTo(2));

            var forStatement = (AST.Statements.For)function!.body[1];
            Assert.That(forStatement!.initialization, Is.InstanceOf<AST.Statements.VariableDeclaration>());
            Assert.That(forStatement!.condition, Is.InstanceOf<AST.Expressions.FunctionCall>());
            Assert.That(forStatement!.increment, Is.InstanceOf<AST.Statements.ValueAssignment>());
            Assert.That(forStatement!.body, Has.Length.EqualTo(1));
        }
    }

    public class InlineAssemblyTest
    {
        [Test]
        public void TestGlobalAdd()
        {
            bool result = Parser.TryParse(@"
            asm {
                pop RD0
                pop RD1
                add RD0, RD1
            };
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.SideEffect>());

            var sideEffect = (AST.Statements.SideEffect)statement;
            Assert.That(sideEffect!.expression, Is.InstanceOf<AST.Expressions.InlineAssembly>());

            var asmStatement = (AST.Expressions.InlineAssembly)sideEffect!.expression;
            Assert.That(asmStatement!.assembly, Has.Length.Not.EqualTo(0));
        }

        [Test]
        public void TestAdd()
        {
            bool result = Parser.TryParse(@"
            function add(var x: i32; var y: i32;): i32
            {
                return asm {
                    pop RD0
                    pop RD1
                    add RD0, RD1
                };
            }
            ",
                out var value,
                out var error,
                out var errorPosition
            );
            Assert.That(error, Is.Null);
            Assert.That(result, Is.True);
            Assert.That(value, Is.InstanceOf<AST.Module>());

            var module = (AST.Module)value;
            Assert.That(module!.statements, Has.Length.EqualTo(1));

            var statement = module!.statements[0];
            Assert.That(statement, Is.InstanceOf<AST.Statements.FunctionDefinition>());

            var function = (AST.Statements.FunctionDefinition)statement;
            Assert.That(function!.name.name, Is.EqualTo("add"));
            Assert.That(function!.arguments, Has.Length.EqualTo(2));
            Assert.That(function!.return_type.ToString(), Is.EqualTo("i32"));
            Assert.That(function!.body, Has.Length.EqualTo(1));

            var returnStatement = (AST.Statements.Return)function!.body[0];
            Assert.That(returnStatement!.value, Is.InstanceOf<AST.Expressions.InlineAssembly>());

            var asmStatement = (AST.Expressions.InlineAssembly)returnStatement!.value;
            Assert.That(asmStatement!.assembly, Is.EqualTo("pop RD0\npop RD1\nadd RD0, RD1"));
        }
    }
}