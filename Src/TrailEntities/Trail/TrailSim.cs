﻿using System.Collections.Generic;
using System.Linq;

namespace TrailEntities
{
    /// <summary>
    ///     Holds all the points of interest that make up the entire trail the players vehicle will be traveling along. Keeps
    ///     track of the vehicles current position on the trail and provides helper methods to quickly access it.
    /// </summary>
    public sealed class TrailSim
    {
        /// <summary>
        ///     Delegate that passes along the next point of interest that was reached to the event and any subscribers to it.
        /// </summary>
        /// <param name="nextPoint">Next point of interest that will be attached to game simulation.</param>
        public delegate void PointOfInterestReached(Location nextPoint);

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Trail" /> class.
        /// </summary>
        /// <param name="trail">Collection of points of interest which make up the trail the player is going to travel.</param>
        public TrailSim(IEnumerable<Location> trail)
        {
            // Builds the trail passed on parameter, sets location to negative one for startup.
            Locations = new List<Location>(trail);
            VehicleLocation = -1;
            DistanceToNextPoint = 0;
        }

        /// <summary>
        ///     Reference to how many ticks are between the players vehicle and the next point of interest.
        /// </summary>
        private ulong DistanceToNextPoint { get; set; }

        /// <summary>
        ///     Current location of the players vehicle as index of points of interest list.
        /// </summary>
        public int VehicleLocation { get; set; }

        /// <summary>
        ///     List of all of the points of interest that make up the entire trail.
        /// </summary>
        public List<Location> Locations { get; }

        /// <summary>
        ///     Advances the vehicle to the next point of interest on the path.
        /// </summary>
        /// <returns>TRUE if we have arrived at next point, FALSE if this method needs called more...</returns>
        public bool MoveTowardsNextPointOfInterest()
        {
            var currentPoint = GetCurrentPointOfInterest();

            // Figure out if this is first step, or one of many after the start.
            if (VehicleLocation == -1)
            {
                // Startup advancement to get things started.
                VehicleLocation = 0;

                // Fire method to do some work and attach game modes based on this.
                OnReachedPointOfInterest(currentPoint);
            }
            else if (VehicleLocation < Locations.Count())
            {
                // Grab some data about our travels on the trail.
                var nextPoint = GetNextPointOfInterest();

                // Check to make sure we are really at the next point based on all available data.
                if (DistanceToNextPoint > 0 || (nextPoint == null || nextPoint.DistanceLength <= 0))
                    return false;

                // Setup next travel distance requirement.
                DistanceToNextPoint = nextPoint.DistanceLength;

                // This is a normal advancement on the trail.
                VehicleLocation++;
                DistanceToNextPoint--;
            }

            // Default response is to return true, up to method to deny access to next point.
            return true;
        }

        /// <summary>
        ///     Locates the next point of interest if it exists in the list, if this method returns NULL then that means the next
        ///     point of interest is the end of the game when the distance to point reaches zero.
        /// </summary>
        public Location GetNextPointOfInterest()
        {
            // Build next point index from current point, even if startup value with -1 we add 1 so will always get first point.
            var nextPointIndex = VehicleLocation + 1;

            // Check if the next point is greater than point count, then get next point of interest if within bounds.
            return nextPointIndex > Locations.Count() ? null : Locations.ElementAt(nextPointIndex);
        }

        /// <summary>
        ///     Returns the current point of interest the players vehicle is on.
        /// </summary>
        public Location GetCurrentPointOfInterest()
        {
            return VehicleLocation <= -1
                ? Locations.First()
                : Locations.ElementAt(VehicleLocation);
        }

        /// <summary>
        ///     Event that will be fired when the next point of interest has been reached on the trail.
        /// </summary>
        public event PointOfInterestReached OnReachPointOfInterest;

        /// <summary>
        ///     Determines if the current point of interest is indeed the first one of the game, makes it easier for game modes and
        ///     states to check this for doing special actions on the first move.
        /// </summary>
        /// <returns>TRUE if first point on trail, FALSE if not.</returns>
        public bool IsFirstPointOfInterest()
        {
            return VehicleLocation <= 0 && GameSimulationApp.Instance.TotalTurns <= 0;
        }

        /// <summary>
        ///     Fired when the players vehicle reaches the next point of interest on the trail.
        /// </summary>
        /// <param name="nextPoint">
        ///     Next point of interest that was peeked in the trail list. If this is null then next point is
        ///     end of game.
        /// </param>
        private void OnReachedPointOfInterest(Location nextPoint)
        {
            // Attach some game mode based on the relevance of the next point type.
            GameSimulationApp.Instance.AddMode(nextPoint.ModeType);

            // Fire event here for API subscribers to know point was reached. 
            OnReachPointOfInterest?.Invoke(nextPoint);
        }
    }
}