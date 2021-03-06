﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public class RuntimeMovieManagerBuilder : MovieManagerBuilder
	{
		public override ICriMovieFileManager Build(RuntimePlatform platform, MovieConfig config)
		{
			var encode = System.Text.Encoding.UTF8;
			var parser = new JsonParser<MovieFileDatabase>(RawFileConverter.Default, encode);
			var saveBuilder = new FileBuilder<CriVersionTable>
			(
				writer : new JsonWriter<CriVersionTable>( RawFileConverter.Default,encode),
				parser: new JsonParser<CriVersionTable>(RawFileConverter.Default, encode)
			);
			return new CriMovieFileManager
			(
				platform:platform,
				config:config,
				engine:new MultiLineJobEngine(1),
				database: null,
				builder:saveBuilder,
				handler : new ErrorHandler()
			);
		}
	}
}