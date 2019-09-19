using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads.CriWare
{

    public interface ISoundFileData
	{
		string CueSheet { get; }

        ICriFileData Acb { get; }
        ICriFileData Awb { get; }
       
		string[] Labels { get; }

		bool HasAwb();
		bool IsInclude();
	}
}
