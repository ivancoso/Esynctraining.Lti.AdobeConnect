namespace Esynctraining.Core.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The inflector.
    /// </summary>
    public static class Inflector
    {
        #region Static Fields

        /// <summary>
        /// The plurals.
        /// </summary>
        private static readonly List<Rule> Plurals = new List<Rule>();

        /// <summary>
        /// The abbreviations.
        /// </summary>
        private static readonly List<string> Abbreviations = new List<string> { "AC", "SN", "AT" };

        /// <summary>
        /// The singulars.
        /// </summary>
        private static readonly List<Rule> Singulars = new List<Rule>();

        /// <summary>
        /// The uncountable array.
        /// </summary>
        private static readonly List<string> Uncountables = new List<string>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Inflector"/> class.
        /// </summary>
        static Inflector()
        {
            AddPlural("$", "s");
            AddPlural("s$", "s");
            AddPlural("(ax|test)is$", "$1es");
            AddPlural("(octop|vir)us$", "$1i");
            AddPlural("(alias|status)$", "$1es");
            AddPlural("(bu)s$", "$1ses");
            AddPlural("(buffal|tomat)o$", "$1oes");
            AddPlural("([ti])um$", "$1a");
            AddPlural("sis$", "ses");
            AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPlural("(hive)$", "$1s");
            AddPlural("([^aeiouy]|qu)y$", "$1ies");
            AddPlural("(x|ch|ss|sh)$", "$1es");
            AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
            AddPlural("([m|l])ouse$", "$1ice");
            AddPlural("^(ox)$", "$1en");
            AddPlural("(quiz)$", "$1zes");

            AddSingular("s$", string.Empty);
            AddSingular("(n)ews$", "$1ews");
            AddSingular("([ti])a$", "$1um");
            AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingular("(^analy)ses$", "$1sis");
            AddSingular("([^f])ves$", "$1fe");
            AddSingular("(hive)s$", "$1");
            AddSingular("(tive)s$", "$1");
            AddSingular("([lr])ves$", "$1f");
            AddSingular("([^aeiouy]|qu)ies$", "$1y");
            AddSingular("(s)eries$", "$1eries");
            AddSingular("(m)ovies$", "$1ovie");
            AddSingular("(x|ch|ss|sh)es$", "$1");
            AddSingular("([m|l])ice$", "$1ouse");
            AddSingular("(bus)es$", "$1");
            AddSingular("(o)es$", "$1");
            AddSingular("(shoe)s$", "$1");
            AddSingular("(cris|ax|test)es$", "$1is");
            AddSingular("(octop|vir)i$", "$1us");
            AddSingular("(alias|status)es$", "$1");
            AddSingular("^(ox)en", "$1");
            AddSingular("(vert|ind)ices$", "$1ex");
            AddSingular("(matr)ices$", "$1ix");
            AddSingular("(quiz)zes$", "$1");

            AddIrregular("person", "people");
            AddIrregular("man", "men");
            AddIrregular("child", "children");
            AddIrregular("sex", "sexes");
            AddIrregular("move", "moves");

            AddUncountable("equipment");
            AddUncountable("information");
            AddUncountable("rice");
            AddUncountable("money");
            AddUncountable("species");
            AddUncountable("series");
            AddUncountable("fish");
            AddUncountable("sheep");
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Make a lower case word in camel case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">
        /// The lowercase and underscored word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Camelize(string lowercaseAndUnderscoredWord)
        {
            return Uncapitalize(Pascalize(lowercaseAndUnderscoredWord));
        }

        /// <summary>
        /// The capitalize.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Capitalize(string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }

        /// <summary>
        /// Replaces underscores with dashes.
        /// </summary>
        /// <param name="underscoredWord">
        /// The underscored word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Dasherize(string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }

        /// <summary>
        /// The humanize.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">
        /// The lowercase and underscored word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Humanize(string lowercaseAndUnderscoredWord)
        {
            return Capitalize(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }

        /// <summary>
        /// Make ordinal string (1st, 2nd) from a number.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Ordinalize(string number)
        {
            int n = int.Parse(number);
            int mod100 = n % 100;

            if (mod100 >= 11 && mod100 <= 13)
            {
                return number + "th";
            }

            switch (n % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        /// <summary>
        /// The pascalize.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">
        /// The lowercase and underscored word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Pascalize(string lowercaseAndUnderscoredWord)
        {
            return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)", match => match.Groups[1].Value.ToUpper());
        }

        /// <summary>
        /// The pluralize.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Pluralize(string word)
        {
            return ApplyRules(Plurals, word);
        }

        /// <summary>
        /// The singularize.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Singularize(string word)
        {
            return ApplyRules(Singulars, word);
        }

        /// <summary>
        /// The titleize.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Titleize(string word)
        {
            return Regex.Replace(Humanize(Underscore(word)), @"\b([a-z])", match => match.Captures[0].Value.ToUpper());
        }

        /// <summary>
        /// Uncapitalize the word.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Uncapitalize(string word)
        {
            string abbr;
            if ((abbr = Abbreviations.FirstOrDefault(word.StartsWith)) != null)
            {
                return abbr.ToLower() + word.Substring(abbr.Length, word.Length - abbr.Length);
            }
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        /// <summary>
        /// The underscore.
        /// </summary>
        /// <param name="pascalCasedWord">
        /// The pascal cased word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Underscore(string pascalCasedWord)
        {
            return
                Regex.Replace(
                    Regex.Replace(
                        Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])", "$1_$2"), 
                    @"[-\s]", 
                    "_").ToLower();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add irregular.
        /// </summary>
        /// <param name="singular">
        /// The singular.
        /// </param>
        /// <param name="plural">
        /// The plural.
        /// </param>
        private static void AddIrregular(string singular, string plural)
        {
            AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
            AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
        }

        /// <summary>
        /// The add plural.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="replacement">
        /// The replacement.
        /// </param>
        private static void AddPlural(string rule, string replacement)
        {
            Plurals.Add(new Rule(rule, replacement));
        }

        /// <summary>
        /// The add singular.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="replacement">
        /// The replacement.
        /// </param>
        private static void AddSingular(string rule, string replacement)
        {
            Singulars.Add(new Rule(rule, replacement));
        }

        /// <summary>
        /// The add uncountable.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        private static void AddUncountable(string word)
        {
            Uncountables.Add(word.ToLower());
        }

        /// <summary>
        /// The apply rules.
        /// </summary>
        /// <param name="rules">
        /// The rules.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ApplyRules(IList<Rule> rules, string word)
        {
            string result = word;

            if (!Uncountables.Contains(word.ToLower()))
            {
                for (int i = rules.Count - 1; i >= 0; i--)
                {
                    if ((result = rules[i].Apply(word)) != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// The rule.
        /// </summary>
        private class Rule
        {
            #region Fields

            /// <summary>
            /// The regex.
            /// </summary>
            private readonly Regex regex;

            /// <summary>
            /// The replacement.
            /// </summary>
            private readonly string replacement;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Rule"/> class.
            /// </summary>
            /// <param name="pattern">
            /// The pattern.
            /// </param>
            /// <param name="replacement">
            /// The replacement.
            /// </param>
            public Rule(string pattern, string replacement)
            {
                this.regex = new Regex(pattern, RegexOptions.IgnoreCase);
                this.replacement = replacement;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The apply.
            /// </summary>
            /// <param name="word">
            /// The word.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public string Apply(string word)
            {
                if (!this.regex.IsMatch(word))
                {
                    return null;
                }

                return this.regex.Replace(word, this.replacement);
            }

            #endregion
        }
    }
}