namespace SysBot.Pokemon
{
    public enum EncounterMode
    {
        /// <summary>
        /// Bot will move back and forth in a straight vertical path to encounter Pokémon
        /// </summary>
        VerticalLine,

        /// <summary>
        /// Bot will move back and forth in a straight horizontal path to encounter Pokémon
        /// </summary>
        HorizontalLine,

        /// <summary>
        /// Bot will move from top left to bottom right in a diagonal path to encounter Pokémon
        /// </summary>
        LeftToRightDiagonal,

        /// <summary>
        /// Bot will move from top right to bottom left in a diagonal path to encounter Pokémon
        /// </summary>
        RightToLeftDiagonal,

        /// <summary>
        /// Bot will soft reset Eternatus
        /// </summary>
        Eternatus,

        /// <summary>
        /// Bot will soft reset the Legendary Dogs
        /// </summary>
        LegendaryDogs,

        /// <summary>
        /// Bot will soft reset any of the Regis, not Regigigas
        /// </summary>
        Regis,

        /// <summary>
        /// Bot will soft reset Regigigas
        /// </summary>
        Regigigas,

        /// <summary>
        /// Bot will soft reset any of the Swords of Justice
        /// </summary>
        SwordsOfJustice,
    }
}