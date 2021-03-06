﻿using System;
using System.Collections.Generic;
using System.Linq;

using CovidSafe.DAL.Helpers;
using CovidSafe.Entities.Geospatial;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CovidSafe.Tests.Helpers
{
    [TestClass]
    public class RegionHelperTests
    {
        void TestRegionBackwardCompatible(double lat, double lon, Region expected)
        {
            var region = RegionHelper.CreateRegion(lat, lon);

            Assert.AreEqual(expected.LatitudePrefix, region.LatitudePrefix);
            Assert.AreEqual(expected.LongitudePrefix, region.LongitudePrefix);
            Assert.AreEqual(expected.Precision, region.Precision);
        }

        [TestMethod]
        public void NYRegionBackwardCompatibilityTest()
        {
            var expected = new Region(40, -73, 8);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
            TestRegionBackwardCompatible(40.73, -73.93, expected);
        }

        void TestRegion(int lat, int lon, int precision, Region expected)
        {
            var region = RegionHelper.AdjustToPrecision(new Region(lat, lon, precision));

            Assert.AreEqual(expected.LatitudePrefix, region.LatitudePrefix);
            Assert.AreEqual(expected.LongitudePrefix, region.LongitudePrefix);
            Assert.AreEqual(expected.Precision, region.Precision);
        }

        [TestMethod]
        public void NewYorkRegionTest()
        {
            TestRegion(40, -73, 0, new Region(0, 0, 0));
            TestRegion(40, -73, 1, new Region(0, 0, 1));
            TestRegion(40, -73, 2, new Region(0, -64, 2));
            TestRegion(40, -73, 3, new Region(32, -64, 3));
            TestRegion(40, -73, 4, new Region(32, -64, 4));
            TestRegion(40, -73, 5, new Region(40, -72, 5));
            TestRegion(40, -73, 6, new Region(40, -72, 6));
            TestRegion(40, -73, 7, new Region(40, -72, 7));
            TestRegion(40, -73, 8, new Region(40, -73, 8));
        }

        private void AssertRegionBounary(RegionBoundary expected, RegionBoundary actual)
        {
            Assert.AreEqual(expected.Min.Latitude, actual.Min.Latitude);
            Assert.AreEqual(expected.Max.Latitude, actual.Max.Latitude);
            Assert.AreEqual(expected.Min.Longitude, actual.Min.Longitude);
            Assert.AreEqual(expected.Max.Longitude, actual.Max.Longitude);
        }

        [TestMethod]
        public void NewYorkRegionBoundaryTest()
        {
            var r = new Region(40, -73);
            {
                r.Precision = 8;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = 40, Longitude = -74 }, 
                    Max = new Coordinates { Latitude = 41, Longitude = -73 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 7;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates {Latitude = 40, Longitude = -74 }, 
                    Max = new Coordinates { Latitude = 42, Longitude = -72 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 6;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = 40, Longitude = -76 }, 
                    Max = new Coordinates { Latitude = 44, Longitude = -72 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 5;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = 40, Longitude = -80 }, 
                    Max = new Coordinates { Latitude = 48, Longitude = -72 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 4;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = 32, Longitude = -80 }, 
                    Max = new Coordinates { Latitude = 48, Longitude = -64 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 3;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = 32, Longitude = -96 }, 
                    Max = new Coordinates { Latitude = 64, Longitude = -64 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 2;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = -64, Longitude = -128 }, 
                    Max = new Coordinates { Latitude = 64, Longitude = -64 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 1;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = -90, Longitude = -128 }, 
                    Max = new Coordinates { Latitude = 90, Longitude = 128 } }, RegionHelper.GetRegionBoundary(r));
            }
            {
                r.Precision = 0;
                AssertRegionBounary(new RegionBoundary { 
                    Min = new Coordinates { Latitude = -90, Longitude = -180 }, 
                    Max = new Coordinates { Latitude = 90, Longitude = 180 } }, RegionHelper.GetRegionBoundary(r));
            }
        }

        [TestMethod]
        public void RegionCoverageTest_Precision0()
        {
            var area = new NarrowcastArea
            {
                BeginTimestamp = 0,
                EndTimestamp = 1,
                RadiusMeters = 1000,
            };


            {
                area.Location = new Coordinates { Latitude = 0.0001, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, 0).ToList();
                Assert.AreEqual(1, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = 89.99999, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, 0).ToList();
                Assert.AreEqual(1, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = -0.0001, Longitude = 179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, 0).ToList();
                Assert.AreEqual(1, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = -89.9999, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, 0).ToList();
                Assert.AreEqual(1, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = -0.0001, Longitude = -179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, 0).ToList();
                Assert.AreEqual(1, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = 89.99999, Longitude = -179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, 0).ToList();
                Assert.AreEqual(1, regions.Count);
            }
        }

        [TestMethod]
        public void RegionCoverageTest_Precision1()
        {
            var area = new NarrowcastArea
            {
                BeginTimestamp = 0,
                EndTimestamp = 1,
                RadiusMeters = 1000,
            };

            int precision = 1;

            {
                area.Location = new Coordinates { Latitude = 0.0001, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(1, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = 89.99999, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(2, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = -0.0001, Longitude = 179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(2, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = -89.9999, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(2, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = -0.0001, Longitude = -179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(2, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = 89.99999, Longitude = -179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(3, regions.Count);
            }
        }

        [TestMethod]
        public void RegionCoverageTest_Precision2()
        {
            var area = new NarrowcastArea
            {
                BeginTimestamp = 0,
                EndTimestamp = 1,
                RadiusMeters = 1000,
            };

            int precision = 2;

            {
                area.Location = new Coordinates { Latitude = 0.0001, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(1, regions.Count);
            }

            /*  Not working. TODO: fix poles
            {
                area.Location = new Coordinates { Latitude = 89.9999, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(3, regions.Count);
            }*/

            {
                area.Location = new Coordinates { Latitude = -0.0001, Longitude = 179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(2, regions.Count);
            }

            /*  Not working. TODO: fix poles
            {
                area.Location = new Coordinates { Latitude = -89.9999, Longitude = 0.0001 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(3, regions.Count);
            }*/

            {
                area.Location = new Coordinates { Latitude = -0.0001, Longitude = -179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(2, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = 89.99999, Longitude = -179.99999 };
                var regions = RegionHelper.GetRegionsCoverage(area, precision).ToList();
                Assert.AreEqual(3, regions.Count);
            }
        }

        [TestMethod]
        public void RegionCoverageTest_Precision_6_7_8()
        {
            var area = new NarrowcastArea
            {
                BeginTimestamp = 0,
                EndTimestamp = 1,
                RadiusMeters = 1000,
            };

            {
                area.Location = new Coordinates { Latitude = 40.99999, Longitude = -73.999999 };
                var regions = RegionHelper.GetRegionsCoverage(area, 8).ToList();
                Assert.AreEqual(4, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = 40.99999, Longitude = -73.999999 };
                var regions = RegionHelper.GetRegionsCoverage(area, 7).ToList();
                Assert.AreEqual(2, regions.Count);
            }

            {
                area.Location = new Coordinates { Latitude = 40.99999, Longitude = -73.999999 };
                var regions = RegionHelper.GetRegionsCoverage(area, 6).ToList();
                Assert.AreEqual(1, regions.Count);
            }
        }
    }
}
