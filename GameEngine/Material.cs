using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    class Material
    {
        public double staticFriction { set; get; }
        public double dynamicFriction { set; get; }

        public double bounciness { set; get; }

        public Material(double staticFriction, double dynamicFiriction, double bounciness) {
            this.bounciness = bounciness;
            this.dynamicFriction = dynamicFriction;
            this.staticFriction = staticFriction;
        }
    }
}
