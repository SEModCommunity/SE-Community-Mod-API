using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEModAPI
{
    public enum ClassType
    {
        Unknown,
        Character,
        FloatingObject,
        LargeShip,
        Station,
        SmallShip,
        Meteor,
        Voxel,
    };

    public enum ImportModelClassType
    {
        SmallShip,
        LargeShip,
        Station,
        Asteroid
    };

    public enum ImportImageClassType
    {
        SmallShip,
        LargeShip,
        Station,
    };

    public enum ImportArmorType
    {
        Light,
        Heavy
    };
}
