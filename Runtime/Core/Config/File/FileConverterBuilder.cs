﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public abstract class FileConverterBuilder : ScriptableObject
	{
		public abstract IFileConverter Build();
	}
}