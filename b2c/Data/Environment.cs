using System.Collections.Generic;
using EPS.Extensions.B2CGraphUtil.Config;

namespace b2c.Data;

public class Environment: GraphUtilConfig
{
    /// <summary>
    /// The environment name.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The <c>Production</c> setting for the schema.
    /// </summary>
    public bool Production { get; set; }

    /// <summary>
    /// The string settings that need replacing in the files.
    /// </summary>
    public Dictionary<string,string> Settings { get; set; }
}
