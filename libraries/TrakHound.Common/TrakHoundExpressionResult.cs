// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundExpressionResult
    {
        public string Namespace { get; }

        public string Expression { get; }

        public string Uuid { get; }


        public TrakHoundExpressionResult(string ns, string expression, string uuid)
        {
            Namespace = ns;
            Expression = expression;
            Uuid = uuid;
        }
    }
}
