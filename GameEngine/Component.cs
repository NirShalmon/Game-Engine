using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// A componnent could be any class coupled with any game object.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// The game object connected to this component.
        /// </summary>
        public readonly GameObject gameObject;
        
        /// <summary>
        /// A component constructor to set the gameobject.
        /// </summary>
        /// <param name="gameObject">The coupled gameobject.</param>
        public Component(GameObject gameObject) {
            this.gameObject = gameObject;
        }


        //TODO add some events.
    }
}
