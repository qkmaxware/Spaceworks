using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Modular {

    /// <summary>
    /// Component compatible with modular builder
    /// </summary>
    public class ModularComponent : MonoBehaviour {
      /// <summary>
      /// Point in which it can connect to other modular components
      /// </summary>
		  public Transform connectionPoint;
    }

}
