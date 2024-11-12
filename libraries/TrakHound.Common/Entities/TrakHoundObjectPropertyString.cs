// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using TrakHound.Requests;

namespace TrakHound.Entities
{
    public static class TrakHoundObjectPropertyString
    {
        public static string Get(string input, TrakHoundObject obj)
        {
            if (!string.IsNullOrEmpty(input) && obj != null)
            {
                var regex = new Regex(@"(\$(.*?)\$)");
                if (regex.IsMatch(input))
                {
                    var matches = regex.Matches(input);
                    if (!matches.IsNullOrEmpty())
                    {
                        var edit = input;

                        foreach (Match match in matches)
                        {
                            if (match.Groups?.Count > 2)
                            {
                                var matchText = match.Groups[1].ToString();
                                var name = match.Groups[2].ToString();

                                switch (name.ToLower())
                                {
                                    case "id": edit = edit.Replace(matchText, obj.Uuid); break;
                                    case "name": edit = edit.Replace(matchText, obj.Path); break;
                                    case "type": edit = edit.Replace(matchText, obj.Type); break;
                                    case "description": edit = edit.Replace(matchText, obj.Description); break;
                                }
                            }
                        }

                        return edit;
                    }
                }

            }

            return input;
        }

        //public static string Get(string input, ITrakHoundObjectEntityModel obj)
        //{
        //    if (!string.IsNullOrEmpty(input) && obj != null)
        //    {
        //        var regex = new Regex(@"(\$(.*?)\$)");
        //        if (regex.IsMatch(input))
        //        {
        //            var matches = regex.Matches(input);
        //            if (!matches.IsNullOrEmpty())
        //            {
        //                var edit = input;

        //                foreach (Match match in matches)
        //                {
        //                    if (match.Groups?.Count > 2)
        //                    {
        //                        var matchText = match.Groups[1].ToString();
        //                        var name = match.Groups[2].ToString();

        //                        switch (name.ToLower())
        //                        {
        //                            case "id": edit = edit.Replace(matchText, obj.Uuid); break;
        //                            case "name": edit = edit.Replace(matchText, obj.Path); break;
        //                            case "type": edit = edit.Replace(matchText, obj.Type); break;
        //                            case "description": edit = edit.Replace(matchText, obj.Description); break;

        //                            //case "parent.id": edit = edit.Replace(matchText, obj.Parent?.Uuid); break;
        //                            //case "parent.name": edit = edit.Replace(matchText, obj.Parent?.Path); break;
        //                            //case "parent.type": edit = edit.Replace(matchText, obj.Parent?.Type); break;
        //                            //case "parent.description": edit = edit.Replace(matchText, obj.Parent?.Description); break;
        //                        }
        //                    }
        //                }

        //                return edit;
        //            }
        //        }

        //    }

        //    return input;
        //}
    }
}
