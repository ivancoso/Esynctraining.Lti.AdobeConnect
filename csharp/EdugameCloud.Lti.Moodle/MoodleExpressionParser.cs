namespace EdugameCloud.Lti.Moodle
{
    using System.Collections.Generic;

    using NCalc;

    /// <summary>
    ///     The moodle formula parser
    /// </summary>
    public sealed class MoodleExpressionParser
    {
        #region Fields

        /// <summary>
        /// The expression.
        /// </summary>
        private readonly Expression expression;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleExpressionParser"/> class. 
        /// Create instance of MoodleExpressionParser
        /// </summary>
        /// <param name="expr">
        /// The expression
        /// </param>
        public MoodleExpressionParser(string expr)
        {
            string convertedString = RemoveCalcBrackets(expr);
            convertedString = ChangeFromMoodleFormat(convertedString);
            this.expression = new Expression(convertedString);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Calculate brackets
        /// </summary>
        /// <param name="expr">
        /// The expression
        /// </param>
        /// <param name="values">
        /// The values
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SimplifyExpression(string expr, Dictionary<string, double> values)
        {
            return CalcBrackets(expr, values);
        }

        /// <summary>
        /// The calculate
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double Calculate()
        {
            return (double)this.expression.Evaluate();
        }

        /// <summary>
        /// The set value
        /// </summary>
        /// <param name="name">
        /// The name
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        public void SetValue(string name, double value)
        {
            this.expression.Parameters[name] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The calculation brackets.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string CalcBrackets(string expression, Dictionary<string, double> values)
        {
            int lastIndex = expression.IndexOf("{=", System.StringComparison.Ordinal);
            if (lastIndex < 0)
            {
                return expression;
            }

            int indexClose = lastIndex + 2;
            int countOpening = 0;
            while (indexClose < expression.Length - 1)
            {
                if (expression[indexClose] == '{')
                {
                    countOpening++;
                }
                else if (expression[indexClose] == '}')
                {
                    if (countOpening == 0)
                    {
                        break;
                    }

                    countOpening--;
                }

                indexClose++;
            }

            string innerValue = expression.Substring(lastIndex + 2, indexClose - lastIndex - 2);

            var parser = new MoodleExpressionParser(innerValue);
            foreach (string key in values.Keys)
            {
                parser.SetValue(key, values[key]);
            }

            return
                CalcBrackets(
                    expression.Substring(0, lastIndex) + parser.Calculate()
                    + (expression.Length > indexClose + 1 ? expression.Substring(indexClose + 1) : string.Empty), 
                    values);
        }

        /// <summary>
        /// The change from moodle format.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ChangeFromMoodleFormat(string expression)
        {
            foreach (
                string func in
                    new[]
                        {
                            "abs", "acos", "asin", "atan", "cos", "exp", "floor", "log", "log10", "max", "min", "pow", 
                            "round", "sin", "sqrt", "tan"
                        })
            {
                expression = expression.Replace(func, func.Substring(0, 1).ToUpper() + func.Substring(1));
            }

            return expression.Replace("ceil", "Ceiling").Replace('{', '[').Replace('}', ']');
        }

        /// <summary>
        /// The remove calculation brackets.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemoveCalcBrackets(string expression)
        {
            int lastIndex = expression.IndexOf("{=", System.StringComparison.Ordinal);
            if (lastIndex < 0)
            {
                return expression;
            }

            int indexClose = lastIndex + 2;
            int countOpening = 0;
            while (indexClose < expression.Length - 1)
            {
                if (expression[indexClose] == '{')
                {
                    countOpening++;
                }
                else if (expression[indexClose] == '}')
                {
                    if (countOpening == 0)
                    {
                        break;
                    }

                    countOpening--;
                }

                indexClose++;
            }

            string innerValue = expression.Substring(lastIndex + 2, indexClose - lastIndex - 2);
            return
                RemoveCalcBrackets(
                    expression.Substring(0, lastIndex) + "(" + innerValue + ")"
                    + (expression.Length > indexClose + 1 ? expression.Substring(indexClose + 1) : string.Empty));
        }

        #endregion
    }
}