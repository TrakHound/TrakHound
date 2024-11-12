// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrakHound.Entities
{
    public static class TrakHoundMetadataString
    {
        public static string Get(string input, IEnumerable<ITrakHoundObjectMetadataEntity> metadatas)
        {
            if (!string.IsNullOrEmpty(input) && !metadatas.IsNullOrEmpty())
            {
                var regex = new Regex(@"(\{(.*?)\})");
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

                                ITrakHoundObjectMetadataEntity metadata = null;

                                if (!metadatas.IsNullOrEmpty())
                                {
                                    metadata = metadatas.FirstOrDefault(o => o.Name == name);
                                    if (metadata != null) edit = edit.Replace(matchText, metadata.Value);
                                }
                            }
                        }

                        return edit;
                    }
                }
            }

            return input;
        }
    }
}
