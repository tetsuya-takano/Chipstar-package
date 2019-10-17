using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public class AssetProviderBuilder : ChipstarAsset
	{
		public virtual IAssetLoadProvider Build(RuntimePlatform platform, AssetBundleConfig config, ILoadDatabase database)
		{
			return new AssetLoadProvider(Factory(database));
		}

		protected virtual IFactoryContainer Factory( ILoadDatabase database )
		{
			return new FactoryContainer
				   (
					   assets: new IAssetLoadFactory[]
					   {
							new AssetBundleLoadFactory( database, 2 ),
							new ResourcesLoadFactory( 1 )
					   },
					   scenes: new ISceneLoadFactory[]
					   {
							new BuiltInSceneLoadFactory( 1 ),
							new SceneLoadFactory( database, 2),
					   }
				   );
		}
	}
}