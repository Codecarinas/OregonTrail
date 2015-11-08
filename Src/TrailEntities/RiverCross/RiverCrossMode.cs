﻿using TrailEntities.Mode;

namespace TrailEntities.RiverCross
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    public sealed class RiverCrossMode : GameMode<RiverCrossCommands>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public RiverCrossMode() : base(false)
        {
            // Create new river crossing information object.
            RiverCrossInfo = new RiverCrossInfo();

            // Add all of the commands for crossing a river.
            AddCommand(FordRiver, RiverCrossCommands.FordRiver, "attempt to ford the river");
            AddCommand(CaulkVehicle, RiverCrossCommands.CaulkVehicle, "caulk the wagon and float it across");
            AddCommand(UseFerry, RiverCrossCommands.UseFerry, "take a ferry across");
            AddCommand(WaitForWeather, RiverCrossCommands.WaitForWeather, "wait to see if conditions improve");
            AddCommand(GetMoreInformation, RiverCrossCommands.GetMoreInformation, "get more information");
        }

        /// <summary>
        ///     Defines all of the information like river depth and ferry cost which will be randomly generated when this object is
        ///     created.
        /// </summary>
        private RiverCrossInfo RiverCrossInfo { get; }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.RiverCrossing; }
        }

        /// <summary>
        ///     Attached a state on top of the river crossing mode to explain what the different options mean and how they work.
        /// </summary>
        private void GetMoreInformation()
        {
            CurrentState = new FordRiverHelpState(this, RiverCrossInfo);
        }

        /// <summary>
        ///     Waits for a day still ticking events but waiting to see if weather will improve and make crossing easier.
        /// </summary>
        private void WaitForWeather()
        {
            CurrentState = new CampByRiverState(this, RiverCrossInfo);
        }

        /// <summary>
        ///     Prompts to pay monies for a ferry operator that will take the vehicle across the river without the danger of user
        ///     trying it themselves.
        /// </summary>
        private void UseFerry()
        {
            RiverCrossInfo.CrossingType = CrossChoice.Ferry;
            CurrentState = new UseFerryConfirmState(this, RiverCrossInfo);
        }

        /// <summary>
        ///     Attempts to float the vehicle over the river to the other side, there is a much higher chance for bad things to
        ///     happen.
        /// </summary>
        private void CaulkVehicle()
        {
            RiverCrossInfo.CrossingType = CrossChoice.Caulk;
            CurrentState = new CrossingResultState(this, RiverCrossInfo);
        }

        /// <summary>
        ///     Rides directly into the river without any special precautions, if it is greater than three feet of water the
        ///     vehicle will be submerged and highly damaged.
        /// </summary>
        private void FordRiver()
        {
            RiverCrossInfo.CrossingType = CrossChoice.Ford;
            CurrentState = new CrossingResultState(this, RiverCrossInfo);
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        /// <param name="modeType"></param>
        protected override void OnModeRemoved(ModeType modeType)
        {
        }
    }
}