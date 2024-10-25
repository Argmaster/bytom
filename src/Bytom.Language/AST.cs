



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
        {
        }

        public class StructDefinition : Statement
        {
            public string name;
            public AliasDeclaration[] fields;

            public StructDefinition(string name, AliasDeclaration[] fields)
            {
                this.name = name;
                this.fields = fields;
            }
        }

        public class BitFieldDefinition : Statement
        {
        }

        public class FunctionDefinition : Statement
        {
            public Expressions.Name name;
            public AliasDeclaration[] arguments;
            public Expressions.TypeName return_type;
            public Statement[] body;
            public FunctionDefinition(
                Expressions.Name name,
                AliasDeclaration[] arguments,
                Expressions.TypeName return_type,
                Statement[] body
            )
            {
                this.name = name;
                this.arguments = arguments;
                this.return_type = return_type;
                this.body = body;
            }
        }

        public interface AliasDeclaration : Statement
        {
            public Expressions.Name name { get; }
            public Expressions.TypeName type { get; }
        }

        public class VariableDeclaration : AliasDeclaration
        {
            public Expressions.Name name { get; }
            public Expressions.TypeName type { get; }
            public Expressions.Expression? value { get; }

            public VariableDeclaration(
                Expressions.Name name,
                Expressions.TypeName type,
                Expressions.Expression? value
            )
            {
                this.name = name;
                this.type = type;
                this.value = value;
            }
        }

        public class ConstantDeclaration : AliasDeclaration
        {
            public Expressions.Name name { get; }
            public Expressions.TypeName type { get; }
            public Expressions.Expression value { get; }

            public ConstantDeclaration(
                Expressions.Name name,
                Expressions.TypeName type,
                Expressions.Expression value
            )
            {
                this.name = name;
                this.type = type;
                this.value = value;
            }
        }

        public class ValueAssignment : Statement
        {
            public Expressions.LeftIdentifier name;
            public Expressions.Expression value;
            public ValueAssignment(
                Expressions.LeftIdentifier name,
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

        public class If : Statement
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

        public class Elif : Statement
        {
            public Expressions.Expression condition;
            public Statement[] body;
            public Elif(
                Expressions.Expression condition,
                Statement[] body
            )
            {
                this.condition = condition;
                this.body = body;
            }
        }

        public class Else : Statement
        {
            public Statement[] body;
            public Else(Statement[] body)
            {
                this.body = body;
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
        {
        }

        public class Cast : Expression
        {
            public TypeName type;
            public Expression value;
            public Cast(TypeName type, Expression value)
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

        public class InlineAssembly : Expression
        {
            public string assembly;
            public InlineAssembly(string assembly)
            {
                this.assembly = assembly;
            }
        }

        public interface LeftIdentifier
        {
        }

        public class Name : Expression, LeftIdentifier
        {
            public string name;
            public Name(string name)
            {
                this.name = name;
            }
        }

        public class DotAccess : Expression, LeftIdentifier
        {
            public Name[] names;
            public DotAccess(Name[] names)
            {
                this.names = names;
            }
        }

        public class TypeName : Expression
        {
            public string type_name;
            public int pointer_level;
            public TypeName(string type_name, int pointer_level)
            {
                this.type_name = type_name;
                this.pointer_level = pointer_level;
            }
        }
    }

}
