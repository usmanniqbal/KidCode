using System;
using System.Collections.Generic;
using System.Text;

namespace KidCode_Compiler
{
	public struct Token
	{
		public readonly string CP;
		public readonly string VP;
		public readonly int LN;

		public Token(string cP, string vP, int lN)
		{
			CP = cP;
			VP = vP;
			LN = lN;
		}
	}
}
