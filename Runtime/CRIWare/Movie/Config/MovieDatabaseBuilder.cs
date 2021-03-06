﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public abstract class MovieDatabaseBuilder : ChipstarAsset
	{
		public abstract IMovieLoadDatabase Build(RuntimePlatform platform, MovieConfig config);
	}
}