﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.Transpiling
{
	class TranspilingException : Exception
	{
		public TranspilingException(string message) : base(message)
		{

		}
	}
}
