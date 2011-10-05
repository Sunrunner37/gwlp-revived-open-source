using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Enums
{
        /// <summary>
        ///   Color of (sender name)_(chat text)
        /// </summary>
        public enum ChatColors
        {
                /// <summary>
                ///   Alliance Chat
                /// </summary>
                Orange_White,

                /// <summary>
                ///   Allied Party Chat
                /// </summary>
                BlueAlly_White,

                /// <summary>
                ///   Broadcasts
                /// </summary>
                LightBlue_LightBlue,

                /// <summary>
                ///   All Chat
                /// </summary>
                Yellow_White,

                /// <summary>
                ///   No Official Usage
                /// </summary>
                Gray_DarkGray,

                /// <summary>
                ///   Gaile's Chat / turquoise
                /// </summary>
                Turquoise_Turquoise,

                /// <summary>
                ///   Emotes
                /// </summary>
                White_White,

                /// <summary>
                ///   No Official Usage
                /// </summary>
                Invisible_Invisible,

                /// <summary>
                ///   The Frog's Chat / turquoise
                /// </summary>
                Gray_Turquoise,

                /// <summary>
                ///   Guild Chat
                /// </summary>
                Green_White,

                /// <summary>
                ///   Announcements
                /// </summary>
                LightGreen_LightGreen,

                /// <summary>
                ///   Team Chat
                /// </summary>
                BlueTeam_White,

                /// <summary>
                ///   Trade Chat
                /// </summary>
                LightPink_LightPink,

                /// <summary>
                ///   Command reply
                /// </summary>
                DarkOrange_DarkOrange,

                /// <summary>
                ///   NPC's Chat
                /// </summary>
                BlueNPC_White
        }
}
