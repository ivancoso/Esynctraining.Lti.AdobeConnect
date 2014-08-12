namespace EdugameCloud.Core.EntityParsing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NCalc;

    /// <summary>
    /// The moodle formula parser
    /// </summary>
    public class MoodleExpressionParser
    {
        private readonly Expression expression;

        /// <summary>
        /// Create instance of MoodleExpressionParser
        /// </summary>
        /// <param name="expr"></param>
        public MoodleExpressionParser(string expr)
        {
            var convertedString = RemoveCalcBrackets(expr);
            convertedString = ChangeFromMoodleFormat(convertedString);
            expression = new Expression(convertedString);
        }

        /// <summary>
        /// Calculate brackets
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <param name="values">The values</param>
        /// <returns></returns>
        public static string SimplifyExpression(string expr, Dictionary<string, double> values)
        {
            return CalcBrackets(expr, values);
        }

        /// <summary>
        /// The set value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, double value)
        {
            expression.Parameters[name] = value;
        }

        /// <summary>
        /// The calculate
        /// </summary>
        /// <returns></returns>
        public double Calculate()
        {
            return (double)expression.Evaluate();
        }

        private static string ChangeFromMoodleFormat(string expression)
        {
            foreach (var func in new []
                                 {
                                     "abs", "acos", "asin", "atan", "cos",
                                     "exp", "floor", "log", "log10", "max", "min",
                                     "pow", "round", "sin", "sqrt", "tan"
                                 })
            {
                expression = expression.Replace(func, func.Substring(0, 1).ToUpper() + func.Substring(1));
            }
            return expression
                .Replace("ceil", "Ceiling")
                .Replace('{', '[')
                .Replace('}', ']');
        }

        private static string RemoveCalcBrackets(string expression)
        {
            var lastIndex = expression.IndexOf("{=");
            if (lastIndex < 0) return expression;
            var indexClose = lastIndex + 2;
            int countOpening = 0;
            while (indexClose < expression.Length - 1)
            {
                if (expression[indexClose] == '{') countOpening++;
                else if (expression[indexClose] == '}')
                {
                    if (countOpening == 0) break;
                    countOpening--;
                }
                indexClose++;
            }
            var innerValue = expression.Substring(lastIndex + 2, indexClose - lastIndex - 2);
            return RemoveCalcBrackets(expression.Substring(0, lastIndex) + "(" + innerValue + ")" + 
                (expression.Length > indexClose + 1 ? expression.Substring(indexClose + 1) : string.Empty));
        }

        private static string CalcBrackets(string expression, Dictionary<string, double> values)
        {
            var lastIndex = expression.IndexOf("{=");
            if (lastIndex < 0) return expression;
            var indexClose = lastIndex + 2;
            int countOpening = 0;
            while (indexClose < expression.Length - 1)
            {
                if (expression[indexClose] == '{') countOpening++;
                else if (expression[indexClose] == '}')
                {
                    if (countOpening == 0) break;
                    countOpening--;
                }
                indexClose++;
            }
            var innerValue = expression.Substring(lastIndex + 2, indexClose - lastIndex - 2);

            var parser = new MoodleExpressionParser(innerValue);
            foreach (var key in values.Keys)
            {
                parser.SetValue(key, values[key]);
            }

            return CalcBrackets(expression.Substring(0, lastIndex) + parser.Calculate() +
                (expression.Length > indexClose + 1 ? expression.Substring(indexClose + 1) : string.Empty), values);
        }
    }
}
