using System;
using System.Diagnostics;

namespace EdugameCloud.Certificates
{
    public static class Check
    {
        public static class Argument
        {
            [DebuggerStepThrough]
            public static void IsNotNullOrEmpty(string argument, string argumentName)
            {
                if (argument == null)
                    throw new ArgumentNullException(argumentName);

                if (argument.Length == 0)
                    throw new ArgumentException(string.Format("\"{0}\" cannot be blank.", argumentName), argumentName);
            }

            [DebuggerStepThrough]
            public static void IsNotNullOrEmpty(Guid argument, string argumentName)
            {
                if (argument == null)
                    throw new ArgumentNullException(argumentName);

                if (argument == Guid.Empty)
                    throw new ArgumentException(string.Format("\"{0}\" cannot be empty guid.", argumentName), argumentName);
            }

            // [ContractAnnotation("argument:null => halt")]
            [DebuggerStepThrough]
            public static void IsNotNull(object argument, string argumentName)
            {
                if (argument == null)
                    throw new ArgumentNullException(argumentName);
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(int argument, string argumentName)
            {
                if (argument < 0)
                    throw new ArgumentOutOfRangeException(argumentName);
            }

            [DebuggerStepThrough]
            public static void IsNotNegative(int? argument, string argumentName)
            {
                if (argument.HasValue && argument < 0)
                    throw new ArgumentOutOfRangeException(argumentName);
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(int argument, string argumentName)
            {
                if (argument <= 0)
                    throw new ArgumentOutOfRangeException(argumentName);
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(int? argument, string argumentName)
            {
                if (argument.HasValue && argument <= 0)
                    throw new ArgumentOutOfRangeException(argumentName);
            }

            [DebuggerStepThrough]
            public static void IsNotNegativeOrZero(decimal argument, string argumentName)
            {
                if (argument <= 0)
                    throw new ArgumentOutOfRangeException(argumentName);
            }

        }

    }
}