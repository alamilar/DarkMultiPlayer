﻿using System;
using UnityEngine;

namespace DarkMultiPlayer
{
    public class VesselUtil
    {
        //Credit where credit is due, Thanks hyperedit.
        public static void CopyOrbit(Orbit sourceOrbit, Orbit destinationOrbit)
        {
            destinationOrbit.inclination = sourceOrbit.inclination;
            destinationOrbit.eccentricity = sourceOrbit.eccentricity;
            destinationOrbit.semiMajorAxis = sourceOrbit.semiMajorAxis;
            destinationOrbit.LAN = sourceOrbit.LAN;
            destinationOrbit.argumentOfPeriapsis = sourceOrbit.argumentOfPeriapsis;
            destinationOrbit.meanAnomalyAtEpoch = sourceOrbit.meanAnomalyAtEpoch;
            destinationOrbit.epoch = sourceOrbit.epoch;
            destinationOrbit.referenceBody = sourceOrbit.referenceBody;
            destinationOrbit.Init();
            destinationOrbit.UpdateFromUT(Planetarium.GetUniversalTime());
        }

        public static double FindGroundHeightAtAltitude(double latitude, double longitude, CelestialBody body)
        {
            //We can only find the ground on bodies that actually *have* ground and if we are in flight near the origin
            if (!HighLogic.LoadedSceneIsFlight || FlightGlobals.fetch.activeVessel == null || body.pqsController == null)
            {
                return -1d;
            }
            //Math functions take radians.
            double latRadians = latitude * Mathf.Deg2Rad;
            double longRadians = longitude * Mathf.Deg2Rad;
            //Radial vector
            Vector3d surfaceRadial = new Vector3d(Math.Cos(latRadians) * Math.Cos(longRadians), Math.Sin(latRadians), Math.Cos(latRadians) * Math.Sin(longRadians));
            double surfaceHeight = body.pqsController.GetSurfaceHeight(surfaceRadial) - body.pqsController.radius;
            Vector3d origin = body.GetWorldSurfacePosition(latitude, longitude, surfaceHeight + 500);
            //Magic numbers!
            LayerMask groundMask = 32769;
            RaycastHit raycastHit;
            //Only return the surface if it's really close.
            if (Vector3d.Distance(FlightGlobals.fetch.activeVessel.GetWorldPos3D(), origin) < 2500)
            {
                //Down vector
                Vector3d downVector = -body.GetSurfaceNVector(latitude, longitude);
                if (Physics.Raycast(origin, downVector, out raycastHit, 1000f, groundMask))
                {
                    double hitAltitude = body.GetAltitude(raycastHit.point);
                    if (!body.ocean || hitAltitude > 0)
                    {
                        return hitAltitude;
                    }
                }
            }
            return -1d;
        }
    }
}

