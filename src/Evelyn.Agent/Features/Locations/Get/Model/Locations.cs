namespace Evelyn.Agent.Features.Locations.Get.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Locations : ReadOnlyCollection<Location>
    {
        public Locations(IList<Location> list) : base(list)
        {
        }

        public static Locations None => new Locations(new List<Location>());
    }
}
