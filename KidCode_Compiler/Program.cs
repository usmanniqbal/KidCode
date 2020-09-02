using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KidCode_Compiler
{
	public struct CP
	{
		public const string ID = nameof(ID);
		public const string DT = nameof(DT);
		public const string NUM_CONST = nameof(NUM_CONST);
		public const string YESNO_CONST = nameof(YESNO_CONST);
		public const string STR_CONST = nameof(STR_CONST);
		public const string EMPTY_CONST = nameof(EMPTY_CONST);
		public const string LE = nameof(LE);
		public const string SCOPE = nameof(SCOPE);
		public const string AO = nameof(AO);
		public const string RO = nameof(RO);

		public const string RBO = "(";
		public const string RBC = ")";

		public const string CBO = "{";
		public const string CBC = "{";

		public const string IF = nameof(IF);
		public const string ELSE = nameof(ELSE);
	}

	class Program
	{
		/// <summary>
		/// Predefined Keywords
		/// </summary>
		static Dictionary<string, string> keywords = new Dictionary<string, string>
		{
			{"start", "start" },
			{"routine", "routine" },
			{"exitroutine", "exitroutine" },
			{"nextroutine", "nextroutine" },
			{"fix", "fix" },
			{"private", CP.SCOPE },
			{"public", CP.SCOPE },
			{"if", "if" },
			{"else", "else" },
			{"type", "type" },
			{"takeEach", "takeEach" },
			{"takeAll", "takeAll" },
			{"finish", "finish" },
			{"panic", "panic" },
			{"return", "return" },
			{"enum", "enum" },
			{"until", "until" },

			{"(", CP.RBO },
			{")", CP.RBC },
			{"{", CP.CBO },
			{"}", CP.CBO },
			{",", "," },

			{"+", "PLUS_MIN" },
			{"-", "PLUS_MIN" },
			{"*", "MUL_DIV" },
			{"/", "MUL_DIV" },

			{">", "RO" },
			{"<", "RO" },
			{"<=", "RO" },
			{">=", "RO" },
			{"==", "RO" },
			{"!=", "RO" },

			{"=", CP.AO },

			{"any", CP.DT },
			{"number", CP.DT },
			{"text", CP.DT },
			{"datetime", CP.DT},
			{"yesNo", CP.DT },
			{"empty", CP.EMPTY_CONST },
		};

		static Dictionary<string, string> classPartRegex = new Dictionary<string, string>
		{
			{ CP.ID, "^[a-zA-Z_][0-9a-zA-Z_]?" },
			{ CP.NUM_CONST, "^[0-9]{1,}(.[0-9]{1,})?$" },
			{ CP.YESNO_CONST, "^yes$|^no$" },
			{ CP.STR_CONST, "^'.*?'$" },
		};

		static void Main(string[] args)
		{
			string sourceCode = File.ReadAllText("../../../SampleCode.txt");
			Tokenize(sourceCode);
		}

		/// <summary>
		/// Generate Tokens for Provided Source Code
		/// </summary>
		/// <param name="sourceCode"></param>
		private static void Tokenize(string sourceCode)
		{
			string[] lines = sourceCode.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			int lineNo = 0;
			List<Token> tokens = new List<Token>();
			const string wordBreakRegex = @"('.*'|'.*'?|>=|<=|==|!=|\r\n|[()\t,\+\-\*\/><={}]|\s)";
			//const string sampleInput = "a ( b ) c \t d , e = f + g - h * i / j > k < l >= m <= n == o != p";
			//var splitedWords = Regex.Split(sampleInput, wordBreakRegex);

			foreach (var line in lines)
			{
				lineNo++;

				var words = Regex.Split(line, wordBreakRegex);

				foreach (var word in words)
				{
					if (string.IsNullOrWhiteSpace(word))
					{
						continue;
					}
					else if (keywords.TryGetValue(word, out var cp))
					{
						tokens.Add(new Token(cp, word, lineNo));
					}
					else
					{
						int tokenCount = tokens.Count;
						foreach (var regex in classPartRegex)
						{
							if (Regex.IsMatch(word, regex.Value))
							{
								tokens.Add(new Token(regex.Key, word, lineNo));
								break;
							}
						}

						if (tokenCount == tokens.Count)
						{
							tokens.Add(new Token(CP.LE, word, lineNo));
						}
					}
				}

				//tokens.Add(new Token(Environment.NewLine, Environment.NewLine, lineNo));
			}

			tokens.Add(new Token("$", "$", lineNo));

			StringBuilder sbTokens = new StringBuilder();
			foreach (var token in tokens)
			{
				string vp;
				string cp;
				//if (token.VP == Environment.NewLine)
				//	vp = cp = "\\r\\n";
				//else if (token.VP == "\t")
				//	vp = cp = "\\t";
				//else
				{
					cp = token.CP;
					vp = token.VP;
				}
				sbTokens.AppendLine($"({cp}, {vp}, {token.LN})");
			}

			File.WriteAllText("../../../Tokens.txt", sbTokens.ToString());

			new CFGParser(tokens).Parse();
		}
	}
}

