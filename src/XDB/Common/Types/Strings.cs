using Discord;

namespace XDB.Common.Types
{
    public class Strings
    {
        //Global Strings
        #region global
        public static readonly string ReleaseVersion = "1.0.5";
        #endregion

        //Common Strings
        #region common
        public static string XDB_Title = $"XDB (rel: {ReleaseVersion})(api: {DiscordConfig.Version})";
        public static string XDB_ConfigLoaded = "[XDB] [Info] Configuration Successfully Loaded!";
        #endregion

        // Keep at bottom
        public static string XDB_Header = @"          
        -----------------------------------------------------------------------------------------------------   
                                             __   _______  ____  
                                             \ \ / /  __ \|  _ \ 
                                              \ V /| |  | | |_) |
                                               > < | |  | |  _ < 
                                              / ^ \| |__| | |_) |
                                             /_/ \_\_____/|____/ 
        -----------------------------------------------------------------------------------------------------
                                                         ";
    }
}
