using System.Collections.Concurrent;
using Jint.Parser.Ast;
using Jint.Runtime.Debugger;

namespace Gsl
{
    internal class ForStatementObserver
    {
        private readonly ConcurrentStack<ForStatement> _forStmtStack = new ConcurrentStack<ForStatement>();

        public int Counter { get; private set; } = 0;

        public ForStatementObserver(Jint.Engine jsEngine)
        {
            jsEngine.Step += OnStepRecordForStatement;
        }

        private StepMode OnStepRecordForStatement(object sender, DebugInformation e)
        {
            if (!_forStmtStack.TryPeek(out var lastForStatement))
            {
                lastForStatement = null;
            }

            if (e.CurrentStatement is ForStatement thisForStatement && thisForStatement != lastForStatement)
            {
                Counter++;
                _forStmtStack.Push(thisForStatement);
            }
            else if ((lastForStatement != null)
                    && (e.CurrentStatement.Range[1] < lastForStatement.Range[0]
                    || lastForStatement.Range[1] < e.CurrentStatement.Range[0]))
            {
                _forStmtStack.TryPop(out var _);
            }
            return StepMode.Into;
        }
    }
}