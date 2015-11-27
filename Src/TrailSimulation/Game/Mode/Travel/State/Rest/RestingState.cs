﻿using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Keeps track of a set number of days and every time the game mode is ticked a day is simulated and days to rest
    ///     subtracted until we are at zero, then the player can close the window but until then input will not be accepted.
    /// </summary>
    [RequiredMode(Mode.Travel)]
    public sealed class RestingState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public RestingState(IModeProduct gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game mode and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var rest = new StringBuilder();
            rest.Append($"{Environment.NewLine}You rest for {UserData.DaysToRest} ");
            rest.Append(UserData.DaysToRest > 1
                ? $"days{Environment.NewLine}{Environment.NewLine}"
                : $"day{Environment.NewLine}{Environment.NewLine}");
            return rest.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            // TODO: Simulate the days to rest in time and event system, this will trigger random event game mode if required.

            // Not accepting user input when resting.
            if (UserData.DaysToRest > 1)
                return;

            // Can only actually stop resting once.
            UserData.DaysToRest = 0;
            ClearState();
        }
    }
}