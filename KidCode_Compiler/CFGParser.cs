using System;
using System.Collections.Generic;
using System.Text;

namespace KidCode_Compiler
{
	public class CFGParser
	{
		private readonly List<Token> _tokens;
		private int t;
		public CFGParser(List<Token> tokens)
		{
			_tokens = tokens;
		}

		/// <summary>
		/// Generate CFG
		/// </summary>
		public void Parse()
		{
			Start();
		}

		private bool Start()
		{
			if (Match("start") && Match(CP.RBO) && Match(CP.RBC))
			{
				if (Match(CP.CBO))
				{
					if (Body())
					{

					}
				}

				if (Match(CP.CBC))
				{
					return true;
				}


				if (Match("$"))
				{

				}
			}

			return false;
		}

		private bool Method()
		{
			if (Match(CP.ID, false) && Match(CP.RBO, false))
			{
				if (Args() && Match(CP.RBC))
				{
					if (Match(CP.CBO))
					{
						if (Body())
						{

						}

						if (Match(CP.CBC))
						{

						}
					}

					if (Match(CP.CBC))
					{
						return true;
					}
				}

				return true;
			}
			return true;
		}

		private bool Body()
		{
			Method();
			Dec_Assign();
			If_Else();

			return false;
		}

		private bool If_Else()
		{
			if (Match(CP.IF, false))
			{
				if (Match(CP.RBO) && Id_Const() && Match(CP.RO, false) && Id_Const() && Match(CP.RBC))
				{
					return Body();
				}

				if (Match(CP.ELSE))
				{
					return If_Else();
				}
				return false;
			}

			return true;
		}

		private bool Args()
		{
			if (Match(CP.DT) && Match(CP.ID))
			{
				if (Match(","))
				{
					if (Args())
					{
						return true;
					}
					return false;
				}
				return true;
			}
			return true;
		}

		private bool Dec_Assign()
		{
			if (Dec())
			{
				Assign();
				Dec_Assign();
				return true;
			}

			return false;
		}
		private bool Dec()
		{
			if (Match(CP.SCOPE, false))
			{
				Match(CP.DT, false);
				if (Match(CP.ID))
				{
					return true;
				}
			}
			else if (NextMatch(CP.AO) && Match(CP.ID, false))
			{
				return true;
			}
			else if (Match(CP.DT, false))
			{
				if (Match(CP.ID))
				{
					return true;
				}
				return false;
			}

			return false;
		}

		private bool Assign()
		{
			if (Match(CP.AO, false))
			{
				if (Id_Const())
				{
					return true;
				}

				return false;
			}

			return true;
		}

		private bool Id_Const()
		{
			if (Match(CP.ID, false) || Const())
			{
				return true;
			}

			return false;
		}

		private bool Const()
		{
			if (Match(CP.NUM_CONST, false) || Match(CP.STR_CONST, false) || Match(CP.YESNO_CONST, false) || Match(CP.EMPTY_CONST, false))
			{
				return true;
			}
			return false;
		}

		private bool Match(string cp, bool throwError = true)
		{
			if (_tokens[t].CP == cp)
			{
				t++;
				return true;
			}
			else
				return Throw(throwError);
		}

		private bool NextMatch(string cp) => _tokens.Count > t && _tokens[t + 1].CP == cp;

		private bool Throw(bool throwError)
		{
			if (throwError)
				throw new Exception($"${_tokens[t].CP} expected at line {_tokens[t].LN}");
			else
				return throwError;
		}
	}
}
