



using System.Linq;

namespace Bytom.Language.AST
{
    public class Module
    {
        public Statements.Statement[] statements;
        public Module(Statements.Statement[] statements)
        {
            this.statements = statements;
        }
    }

    namespace Statements
    {
        public interface Statement
        { }

        public interface NamedDefinition : Statement
        {
            public string GetName();
        }

        public class StructDefinition : NamedDefinition
        {
            public Expressions.Name name;
            public ConstantDeclaration[] constants;
            public VariableDeclaration[] variables;
            public FunctionDefinition[] methods;

            public StructDefinition(
                Expressions.Name name,
                ConstantDeclaration[] constants,
                VariableDeclaration[] variables,
                FunctionDefinition[] methods
            )
            {
                this.name = name;
                this.constants = constants;
                this.variables = variables;
                this.methods = methods;
            }
            public string GetName()
            {
                return name.name;
            }
        }

        public interface StructMember : Statement
        { }

        public class BitFieldDefinition : Statement
        {
        }

        public class FunctionDefinition : NamedDefinition
        {
            public Expressions.Name name;
            public AliasDeclaration[] arguments;
            public Expressions.TypeIdentifier return_type;
            public Statement[] body;
            public FunctionDefinition(
                Expressions.Name name,
                AliasDeclaration[] arguments,
                Expressions.TypeIdentifier return_type,
                Statement[] body
            )
            {
                this.name = name;
                this.arguments = arguments;
                this.return_type = return_type;
                this.body = body;
            }
            public string GetName()
            {
                return name.name;
            }
            public string GetReturnTypeName()
            {
                return return_type.ToString();
            }
        }

        public class AliasDeclaration : NamedDefinition
        {
            public Expressions.Name name { get; }
            public Expressions.TypeIdentifier type { get; }

            public AliasDeclaration(
                Expressions.Name name,
                Expressions.TypeIdentifier type
            )
            {
                this.name = name;
                this.type = type;
            }
            public string GetName()
            {
                return name.name;
            }
            public string GetTypeName()
            {
                return type.ToString();
            }
        }

        public class VariableDeclaration : AliasDeclaration
        {
            public Expressions.Expression? value { get; }

            public VariableDeclaration(
                Expressions.Name name,
                Expressions.TypeIdentifier type,
                Expressions.Expression? value
            ) : base(name, type)
            {
                this.value = value;
            }
        }

        public class ConstantDeclaration : AliasDeclaration
        {
            public Expressions.Expression value { get; }

            public ConstantDeclaration(
                Expressions.Name name,
                Expressions.TypeIdentifier type,
                Expressions.Expression value
            ) : base(name, type)
            {
                this.value = value;
            }
        }

        public class ValueAssignment : Statement
        {
            public Expressions.NameIdentifier name;
            public Expressions.Expression value;
            public ValueAssignment(
                Expressions.NameIdentifier name,
                Expressions.Expression value
            )
            {
                this.name = name;
                this.value = value;
            }
        }

        public class Return : Statement
        {
            public Expressions.Expression? value;
            public Return(Expressions.Expression? value)
            {
                this.value = value;
            }
        }

        public class While : Statement
        {
            public Expressions.Expression condition;
            public Statement[] body;
            public While(
                Expressions.Expression condition,
                Statement[] body
            )
            {
                this.condition = condition;
                this.body = body;
            }
        }

        public class For : Statement
        {
            public VariableDeclaration initialization;
            public Expressions.Expression condition;
            public ValueAssignment increment;
            public Statement[] body;
            public For(
                VariableDeclaration initialization,
                Expressions.Expression condition,
                ValueAssignment increment,
                Statement[] body
            )
            {
                this.initialization = initialization;
                this.condition = condition;
                this.increment = increment;
                this.body = body;
            }
        }

        public class If
        {
            public Expressions.Expression condition;
            public Statement[] body;
            public If(
                Expressions.Expression condition,
                Statement[] body
            )
            {
                this.condition = condition;
                this.body = body;
            }
        }

        public class Else
        {
            public Statement[] body;
            public Else(Statement[] body)
            {
                this.body = body;
            }
        }

        public class Conditional : Statement
        {
            public If if_;
            public If[] elif_;
            public Else? else_;
            public Conditional(
                If if_,
                If[] elif_,
                Else? else_
            )
            {
                this.if_ = if_;
                this.elif_ = elif_;
                this.else_ = else_;
            }
        }

        public class SideEffect : Statement
        {
            public Expressions.Expression expression;
            public SideEffect(Expressions.Expression expression)
            {
                this.expression = expression;
            }
        }
    }

    namespace Expressions
    {

        public interface Expression
        { }

        public class Cast : Expression
        {
            public TypeIdentifier type;
            public Expression value;
            public Cast(TypeIdentifier type, Expression value)
            {
                this.type = type;
                this.value = value;
            }
        }

        public class FunctionCall : Expression
        {
            public Name name;
            public Expression[] arguments;
            public FunctionCall(Name name, Expression[] arguments)
            {
                this.name = name;
                this.arguments = arguments;
            }
        }

        public class Literal : Expression
        {
        }

        public class StringLiteral : Literal
        {
            public string value;
            public StringLiteral(string value)
            {
                this.value = value;
            }
        }

        public class IntegerLiteral : Literal
        {
            public long value;
            public IntegerLiteral(long value)
            {
                this.value = value;
            }
        }

        public class InlineAssembly : Expression
        {
            public string assembly;
            public InlineAssembly(string assembly)
            {
                this.assembly = assembly;
            }
        }

        public interface NameIdentifier
        {
        }

        public class Name : Expression, NameIdentifier
        {
            public string name { get; }
            public Name(string name)
            {
                this.name = name;
            }
        }

        public class DotAccess : Expression, NameIdentifier
        {
            public NameIdentifier first { get; }
            public NameIdentifier rest { get; }
            public DotAccess(NameIdentifier first, NameIdentifier rest)
            {
                this.first = first;
                this.rest = rest;
            }
        }

        public interface TypeIdentifier
        {
            public long GetPointerLevel()
            {
                return 0;
            }
        }

        public class TypeName : TypeIdentifier
        {
            public string name { get; }
            public TypeName(string name)
            {
                this.name = name;
            }
            public override string ToString()
            {
                return name;
            }
        }

        public class TypeDotAccess : TypeIdentifier
        {
            public TypeIdentifier first { get; }
            public TypeIdentifier second { get; }
            public TypeDotAccess(TypeIdentifier first, TypeIdentifier second)
            {
                this.first = first;
                this.second = second;
            }
            public override string ToString()
            {
                return first.ToString() + "." + second.ToString();
            }
        }

        public class GenericTypeName : TypeIdentifier
        {
            public string name { get; }
            public GenericTypeName(string name)
            {
                this.name = name;
            }
        }

        public class GenericTypeSpecialization : TypeIdentifier
        {
            public TypeIdentifier generic { get; }
            public TypeIdentifier[] specialization { get; }
            public GenericTypeSpecialization(TypeIdentifier generic, TypeIdentifier[] specialization)
            {
                this.generic = generic;
                this.specialization = specialization;
            }
            public override string ToString()
            {
                return generic + "<" + string.Join(
                    ", ", specialization.Select(s => s.ToString())
                ) + ">";
            }
        }

        public class PointerType : TypeIdentifier
        {
            public TypeIdentifier identifier { get; }
            public PointerType(TypeIdentifier identifier)
            {
                this.identifier = identifier;
            }
            public override string ToString()
            {
                return identifier.ToString() + "*";
            }
            public long GetPointerLevel()
            {
                return 1 + identifier.GetPointerLevel();
            }
        }
    }
}
