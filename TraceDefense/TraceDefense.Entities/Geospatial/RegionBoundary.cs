﻿using System;
using TraceDefense.Entities.Protos;

namespace TraceDefense.Entities.Geospatial
{
    /// <summary>
    /// Defines boundaries of a <see cref="Region"/>
    /// </summary>
    public class RegionBoundary
    {
        /// <summary>
        /// Maximum <see cref="Region"/> boundary coordinates
        /// </summary>
        public Location Max { get; set; }
        /// <summary>
        /// Maximum <see cref="Region"/> boundary coordinates
        /// </summary>
        public Location Min { get; set; }

        /// <summary>
        /// Creates a new <see cref="RegionBoundary"/> from a provided <see cref="Region"/>
        /// </summary>
        /// <param name="region">Source <see cref="Region"/></param>
        /// <remarks>
        /// TODO: Create actual logic for this.
        /// </remarks>
        public static RegionBoundary FromRegion(Region region)
        {
            if(region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            double delta = Math.Pow(0.1, region.Precision);
            return new RegionBoundary
            {
                Min = new Location { Lattitude = (float)(region.LattitudePrefix - delta), Longitude = (float)(region.LattitudePrefix - delta) },
                Max = new Location { Lattitude = (float)(region.LattitudePrefix + delta), Longitude = (float)(region.LattitudePrefix + delta) }
            };
        }
    }
}
